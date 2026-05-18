import os
import sys
import re
import json
import sqlite3

def init_db(db_path):
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    # Check if tables exist, if not create them
    cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND name='modules'")
    if not cursor.fetchone():
        schema_path = os.path.join(os.path.dirname(__file__), "..", "db", "schema.sql")
        if os.path.exists(schema_path):
            with open(schema_path, "r", encoding="utf-8") as f:
                cursor.executescript(f.read())
            conn.commit()
    return conn

def extract_function_body(text, start_pos):
    brace_count = 0
    for i in range(start_pos, len(text)):
        ch = text[i]
        if ch == '{':
            brace_count += 1
        elif ch == '}':
            brace_count -= 1
            if brace_count == 0:
                return text[start_pos+1:i], i
    return None, len(text)

def run_sync():
    print("Syncing .ntv files to database...")
    
    # Locate database
    db_path = "db/netiv.db"
    if not os.path.exists("db"):
        os.makedirs("db", exist_ok=True)
    
    conn = init_db(db_path)
    cursor = conn.cursor()
    
    src_dir = "src"
    if not os.path.exists(src_dir):
        print(f"Error: {src_dir}/ directory not found.")
        sys.exit(1)
        
    # Traverse src and library folders to sync everything
    scan_paths = [src_dir]
    lib_dir = "lib"
    if os.path.exists(lib_dir):
        scan_paths.append(lib_dir)
        
    ntv_files = []
    for scan_path in scan_paths:
        for root, dirs, files in os.walk(scan_path):
            for file in files:
                if file.endswith(".ntv"):
                    ntv_files.append(os.path.join(root, file))
                    
    for file_path in ntv_files:
        normalized_path = file_path.replace("\\", "/")
        print(f"Processing {normalized_path}...")
        
        with open(file_path, "r", encoding="utf-8") as f:
            content = f.read()
            
        file_stem = os.path.splitext(os.path.basename(file_path))[0]
        
        # 1. Parse <meta> to extract module name
        meta_match = re.search(r'<meta>\s*({.*?})\s*;', content, re.DOTALL)
        module_name = file_stem
        if meta_match:
            try:
                meta_json = meta_match.group(1)
                meta_data = json.loads(meta_json)
                module_name = meta_data.get("name", file_stem)
            except Exception:
                pass
                
        # Ensure module exists
        cursor.execute("INSERT OR IGNORE INTO modules (name, path) VALUES (?, ?)", (module_name, normalized_path))
        cursor.execute("SELECT id FROM modules WHERE name = ?", (module_name,))
        module_id = cursor.fetchone()[0]
        
        # 2. Extract <book> section
        book_match = re.search(r'<book>\s*{(.*)}', content, re.DOTALL)
        if book_match:
            book_content = book_match.group(1)
            
            # Find all functions: °function •name(args) -> ret { or °method •name(...)
            func_pattern = re.compile(r'(?:°function|°method)\s+•([a-zA-Z0-9_]+)\s*\((.*?)\)\s*(?:->\s*([^\s{]+))?\s*\{')
            
            for match in func_pattern.finditer(book_content):
                func_name = match.group(1)
                func_args = match.group(2).strip()
                func_ret = (match.group(3) or "°void").strip()
                
                # Extract matching body
                start_bracket = match.end() - 1
                func_body, _ = extract_function_body(book_content, start_bracket)
                
                if func_body is not None:
                    # Upsert function
                    cursor.execute("""
                        INSERT OR REPLACE INTO functions (module_id, name, signature, return_type, visibility)
                        VALUES (?, ?, ?, ?, 'public')
                    """, (module_id, func_name, f"({func_args})", func_ret))
                    
                    cursor.execute("SELECT id FROM functions WHERE module_id = ? AND name = ?", (module_id, func_name))
                    function_id = cursor.fetchone()[0]
                    
                    # Delete existing blocks and statements
                    cursor.execute("DELETE FROM statements WHERE block_id IN (SELECT id FROM blocks WHERE function_id = ?)", (function_id,))
                    cursor.execute("DELETE FROM blocks WHERE function_id = ?", (function_id,))
                    
                    # Insert body block
                    cursor.execute("INSERT INTO blocks (function_id, kind) VALUES (?, 'body')", (function_id,))
                    cursor.execute("SELECT id FROM blocks WHERE function_id = ? AND kind = 'body'", (function_id,))
                    block_id = cursor.fetchone()[0]
                    
                    # Insert statements line-by-line
                    lines = func_body.splitlines()
                    ordinal = 0
                    for line in lines:
                        trimmed = line.strip()
                        if not trimmed:
                            continue
                        cursor.execute("INSERT INTO statements (block_id, statement_text, ordinal) VALUES (?, ?, ?)", (block_id, trimmed, ordinal))
                        ordinal += 1
                        
                    print(f"  Synced function: {func_name}")
                    
    conn.commit()
    conn.close()
    print("Sync complete.")

def run_check():
    print("Checking project for errors...")
    
    db_path = "db/netiv.db"
    if not os.path.exists(db_path):
        print("Error: Database db/netiv.db not found. Run sync first.")
        sys.exit(1)
        
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    log_lines = []
    log_lines.append("Netiv Unified Check Log")
    log_lines.append("======================\n")
    
    # 1. Check all modules
    cursor.execute("SELECT id, name, path FROM modules")
    modules = cursor.fetchall()
    
    errors = 0
    for mod_id, mod_name, mod_path in modules:
        log_lines.append(f"Checking module: {mod_name} ({mod_path})... OK")
        
        # Check functions for this module
        cursor.execute("SELECT id, name, signature, return_type FROM functions WHERE module_id = ?", (mod_id,))
        funcs = cursor.fetchall()
        
        for fn_id, fn_name, fn_sig, fn_ret in funcs:
            # Check statements
            cursor.execute("""
                SELECT statement_text FROM statements 
                WHERE block_id IN (SELECT id FROM blocks WHERE function_id = ?)
                ORDER BY ordinal
            """, (fn_id,))
            stmts = cursor.fetchall()
            
            full_body = "\n".join([s[0] for s in stmts])
            
            # Simple syntax checks on full function body
            if full_body.count("(") != full_body.count(")"):
                log_lines.append(f"  [Error] Function {fn_name}: Unmatched parentheses across body.")
                errors += 1
            if full_body.count("{") != full_body.count("}"):
                log_lines.append(f"  [Error] Function {fn_name}: Unmatched braces across body.")
                errors += 1
                    
    log_lines.append(f"\nProject check completed with {errors} errors.")
    
    # Save log
    if not os.path.exists("build"):
        os.makedirs("build", exist_ok=True)
    with open("build/check.log", "w", encoding="utf-8") as f:
        f.write("\n".join(log_lines))
        
    print("\n".join(log_lines))
    conn.close()
    
    if errors > 0:
        sys.exit(1)

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python sync.py [sync|check]")
        sys.exit(1)
        
    cmd = sys.argv[1].lower()
    if cmd == "sync":
        run_sync()
    elif cmd == "check":
        run_check()
    else:
        print(f"Unknown command: {cmd}")
        sys.exit(1)
