# C Examples Native Conversion & Integration Backlog

This backlog maps out the complete migration path for converting our curated C example files into native Netiv standard libraries. Tasks are ranked **by source file size** (from smallest/simplest to largest/most complex).

---

## 💎 The Conversion TODO Backlog (Ranked by Size)

### [ ] Rank 1: Port `varint.c` to `std.ds` (Variable Integers)
* **Size**: **1,101 bytes** (Easiest / Smallest)
* **Goal**: Enable highly compressed, variable-length integer serialization for Netiv binary stream parsers.
* **Target File**: `src/library/std/std_ds.ntv`
* **Namespace**: `std.ds`
* **Execution Blueprint**:
  1. Define structural wrappers in `<meta>` describing the custom serialization layouts.
  2. Implement `°function •encode_varint(•buf: °pointer<°u8>, •bufsize: °usize, •value: °usize) -> °usize`.
  3. Implement `°function •decode_varint(•bufp: °pointer<°const °u8>, •varint_len: °pointer<°usize>) -> °usize`.
  4. Port standard overflow and bitwise shift checks (`<< 7`, `>> 7`, and MSB masks).
  5. Add test coverage in `src/library/std/std_test_spec_101.ntv` to ensure correct serialization and round-tripping of values up to 64 bits.

### [ ] Rank 2: Port `adler32.c` to `std.compress` (Adler Checksum)
* **Size**: **5,128 bytes** (Second smallest)
* **Goal**: Provide lightning-fast data block verification stubs for standard memory streams and file validation.
* **Target File**: `src/library/std/std_compress.ntv`
* **Namespace**: `std.compress`
* **Execution Blueprint**:
  1. Define a prime modulus base constant (`BASE = 65521`) inside `<book>` via direct integer wrappers.
  2. Translate Zlib's unrolled loop optimizations (`DO16`) into standard label-based branch iterations (`°label`, `°goto`).
  3. Implement `°function •adler32(•adler: °usize, •buf: °pointer<°const °u8>, •len: °usize) -> °usize`.

### [ ] Rank 3: Port `wildmatch.c` to `std.wildmatch` (Wildcard Globbing)
* **Size**: **9,769 bytes** (Third smallest)
* **Goal**: Empower the compiler and CLI with powerful wildcard globbing patterns (`*`, `?`, `[]`) to handle recursive source file crawling natively.
* **Target File**: `src/library/std/std_wildmatch.ntv`
* **Namespace**: `std.wildmatch`
* **Execution Blueprint**:
  1. Implement a character comparison routine `°function •wildmatch(•pattern: °pointer<°const °u8>, •text: °pointer<°const °u8>) -> °bool`.
  2. Translate recursive pointer comparisons to scan character by character:
     * Check for literal matches.
     * Check for single character glob matches (`?`).
     * Check for multi-character glob matches (`*`), branching into standard lookahead iterations.

### [ ] Rank 4: Port `crc32.c` to `std.compress` (Cyclic Redundancy Hashing)
* **Size**: **30,718 bytes** (Medium size)
* **Goal**: Enable standard 32-bit CRC validation for file formats and SQLite database page crawlers.
* **Target File**: `src/library/std/std_compress.ntv` (Appended)
* **Namespace**: `std.compress`
* **Execution Blueprint**:
  1. Implement a pre-calculated 256-word CRC table compiled into unmanaged static memory.
  2. Implement `°function •crc32(•crc: °usize, •buf: °pointer<°const °u8>, •len: °usize) -> °usize`.
  3. Integrate checksum validation checks inside standard file utilities in `std_io.ntv`.

### [ ] Rank 5: Port Minizip Decompression (`miniunz.c` / `unzip.c`) to `std.zip` (Archive Extractors)
* **Size**: **88,508 bytes** combined (Large size)
* **Goal**: Enable the native Netiv compiler to extract library modules, packages, and toolchains directly from zipped distributions.
* **Target File**: `src/library/std/std_zip.ntv`
* **Namespace**: `std.zip`
* **Execution Blueprint**:
  1. Port the lightweight decompression dictionary builders (`inftrees.c` and `inflate.c`) into pure Netiv functions.
  2. Implement `°function •zip_open(•path: °pointer<°const °u8>) -> °pointer<°void>` and `°function •zip_extract_file(•zip_handle: °pointer<°void>, •filename: °pointer<°const °u8>, •dest_path: °pointer<°const °u8>) -> °bool`.
  3. Bind standard unmanaged stream reads (`syscall 11`) to read archives directly into the decompression buffers.

### [ ] Rank 6: Port GLFW Windowing (`rcore_desktop_win32.c`) to `adj.nray` (Physical Display UI)
* **Size**: **92,680 bytes** (Largest / Most complex)
* **Goal**: Move Netiv beyond text consoles to initialize direct desktop UI rendering screens and capture input events natively.
* **Target File**: `src/library/adj/nray/nray_window.ntv`
* **Namespace**: `adj.nray.window`
* **Execution Blueprint**:
  1. Port GLFW/Win32 structure mappings including window handle binds (`HWND`, `HDC`), display modes, and pixel formats.
  2. Implement `°function •nray_init_window(•width: °usize, •height: °usize, •title: °pointer<°const °u8>) -> °pointer<°void>`.
  3. Implement `°function •nray_poll_events() -> °void` to route mouse clicks, window resizes, and keystrokes directly into Netiv event handlers.

---

## 🚀 Line-by-Line Code Translation Blueprint (`varint.c`)

To guide these migrations, use the validated line-by-line translation pattern below:

### Original C:
```c
uintmax_t git_decode_varint(const unsigned char *bufp, size_t *varint_len)
{
    const unsigned char *buf = bufp;
    unsigned char c = *buf++;
    uintmax_t val = c & 127;
    while (c & 128) {
        val += 1;
        if (!val || MSB(val, 7)) {
            *varint_len = 0;
            return 0; /* overflow */
        }
        c = *buf++;
        val = (val << 7) + (c & 127);
    }
    *varint_len = buf - bufp;
    return val;
}
```

### Translated Netiv:
```netiv
°function •decode_varint(•bufp: °pointer<°const °u8>, •varint_len: °pointer<°usize>) -> °usize {
  °unsafe {
      °pointer<°u8> •buf
      •buf = •bufp
      
      °u8 •c
      •c = *•buf
      •buf = •buf + 1
      
      °usize •val
      •val = •c & 127
      
      °label loop_start
      °usize •has_more
      •has_more = •c & 128
      °if (•has_more == 0) {
          °goto loop_done
      }
      
      •val = •val + 1
      
      // Check for overflow
      °if (•val == 0) {
          *•varint_len = 0
          rax = 0
          return
      }
      
      •c = *•buf
      •buf = •buf + 1
      
      •val = (•val << 7) + (•c & 127)
      °goto loop_start
      
      °label loop_done
      *•varint_len = •buf - •bufp
      rax = •val
      return
  }
}
```
