import sqlite3

def dump_schema():
    conn = sqlite3.connect('db/netiv.db')
    cursor = conn.cursor()
    cursor.execute("SELECT sql FROM sqlite_master WHERE type='table'")
    for row in cursor.fetchall():
        if row[0]:
            print(row[0] + ";\n")
    conn.close()

if __name__ == '__main__':
    dump_schema()
