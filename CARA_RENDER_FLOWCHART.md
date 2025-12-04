# Cara Render Flowchart Engineering Management System

File `flowchart_engineering.mmd` berisi 8 flowchart untuk sistem Engineering Management, dibuat menggunakan **Mermaid syntax**.

## 📋 Daftar Flowchart yang Tersedia

1. **Login & Authentication** - Proses autentikasi pengguna
2. **ECN/CCN Management** - Pembuatan dan approval ECN/CCN
3. **Activity & Task Management** - Pembuatan dan tracking task dengan notification flow
4. **BOM Approval Process** - Proses approval BOM dengan multiple PICs
5. **Support Document Management** - Upload dan linking dokumen pendukung
6. **AI Document Analyzer** - Analisis dokumen menggunakan AI
7. **Dashboard Metrics** - Tampilan dan update dashboard metrics
8. **Reporting & Export** - Generate dan export laporan ke Excel

## 🎨 Cara Render Menjadi Gambar

### **Metode 1: Online Editor (Paling Mudah)** ⭐ RECOMMENDED

1. Buka https://mermaid.live
2. Copy isi file `flowchart_engineering.mmd`
3. Paste ke editor
4. Klik "Download PNG" atau "Download SVG"
5. Pilih flowchart mana yang ingin dirender (atau render semua)

**Tips:** Untuk render satu persatu, copy hanya bagian flowchart yang diinginkan (misal: hanya section "1. Login & Authentication")

### **Metode 2: VS Code Extension**

1. Install extension di VS Code:
   - "Markdown Preview Mermaid Support" atau
   - "Mermaid Preview"
2. Buka file `flowchart_engineering.mmd`
3. Klik kanan → "Preview" atau tekan `Ctrl+Shift+V`
4. Export sebagai PNG/SVG dari preview

### **Metode 3: Command Line (Advanced)**

```bash
# Install mermaid-cli
npm install -g @mermaid-js/mermaid-cli

# Render semua flowchart
mmdc -i flowchart_engineering.mmd -o flowchart_engineering.png

# Atau render dengan background putih
mmdc -i flowchart_engineering.mmd -o flowchart_engineering.png -b white

# Render ke SVG (scalable)
mmdc -i flowchart_engineering.mmd -o flowchart_engineering.svg
```

### **Metode 4: GitHub/GitLab**

Jika repository Anda di GitHub/GitLab:
1. Upload file `flowchart_engineering.mmd`
2. Rename menjadi `flowchart_engineering.md` (tambahkan wrapper):
   ```markdown
   # Flowchart Engineering System

   ```mermaid
   [isi flowchart disini]
   ```
   ```
3. GitHub/GitLab akan otomatis render mermaid diagram

### **Metode 5: Online Tools Lainnya**

- **Mermaid Chart**: https://www.mermaidchart.com/
- **Kroki**: https://kroki.io/
- **Draw.io with Mermaid plugin**

## 🎯 Tips Render yang Bagus

### Untuk Presentasi:
```bash
# Render dengan ukuran lebih besar dan background putih
mmdc -i flowchart_engineering.mmd -o flowchart.png -w 2400 -b white
```

### Untuk Dokumentasi:
```bash
# Render ke SVG (kualitas terbaik, scalable)
mmdc -i flowchart_engineering.mmd -o flowchart.svg
```

### Untuk Website:
```bash
# Render dengan transparent background
mmdc -i flowchart_engineering.mmd -o flowchart.png -b transparent
```

## 📝 Cara Memisahkan Flowchart per Bagian

Jika Anda ingin render setiap flowchart secara terpisah:

1. Buka file `flowchart_engineering.mmd`
2. Copy salah satu section (misal: section "1. Login & Authentication")
3. Simpan ke file baru: `flowchart_1_login.mmd`
4. Render file tersebut

**Contoh:** Untuk render hanya Login flow:
```bash
# Buat file baru
nano flowchart_1_login.mmd

# Copy hanya section 1, lalu:
mmdc -i flowchart_1_login.mmd -o flowchart_login.png -b white
```

## 🎨 Kustomisasi Warna dan Style

Anda bisa menambahkan style di file `.mmd` dengan menambahkan:

```mermaid
%%{init: {
  'theme': 'base',
  'themeVariables': {
    'primaryColor': '#4A90E2',
    'primaryTextColor': '#fff',
    'primaryBorderColor': '#2E5C8A',
    'lineColor': '#2E5C8A',
    'secondaryColor': '#7CB342',
    'tertiaryColor': '#FFA726'
  }
}}%%
```

Tambahkan di awal setiap flowchart untuk mengubah warna.

## 📦 Alternatif: Export dari Mermaid.live

Langkah detail untuk pemula:

1. Buka https://mermaid.live
2. Hapus semua kode default di editor (panel kiri)
3. Copy **HANYA** satu flowchart dari file `flowchart_engineering.mmd`
   - Contoh: Copy dari `graph TB` sampai `end` untuk section "1. Login & Authentication"
4. Paste ke editor di Mermaid.live
5. Flowchart akan otomatis muncul di panel kanan
6. Klik icon "Download" di toolbar atas
7. Pilih format:
   - **PNG** - untuk presentasi/dokumen
   - **SVG** - untuk kualitas terbaik
   - **PDF** - untuk print

Ulangi untuk setiap section yang ingin Anda render.

## 🔧 Troubleshooting

**Masalah: Flowchart terlalu besar tidak muat di satu halaman**
- Solusi: Render setiap section secara terpisah

**Masalah: Teks terlalu kecil**
- Solusi: Gunakan parameter `-w 3000` untuk width lebih besar
  ```bash
  mmdc -i file.mmd -o output.png -w 3000
  ```

**Masalah: Background hitam**
- Solusi: Tambahkan `-b white`
  ```bash
  mmdc -i file.mmd -o output.png -b white
  ```

**Masalah: Command mmdc not found**
- Solusi: Install mermaid-cli terlebih dahulu
  ```bash
  npm install -g @mermaid-js/mermaid-cli
  ```

## 📚 Resources

- Mermaid Documentation: https://mermaid.js.org/
- Mermaid Live Editor: https://mermaid.live
- Mermaid Examples: https://mermaid.js.org/syntax/examples.html
- Syntax Guide: https://mermaid.js.org/intro/

## ✅ Quick Start (Termudah)

**Untuk pengguna non-technical:**

1. Buka https://mermaid.live
2. Buka file `flowchart_engineering.mmd` dengan text editor
3. Copy section "1. Login & Authentication" (dari `graph TB` sampai akhir section)
4. Paste di mermaid.live
5. Klik "Download PNG"
6. Selesai! Ulangi untuk section lain

**Untuk developer:**

```bash
npm install -g @mermaid-js/mermaid-cli
mmdc -i flowchart_engineering.mmd -o flowchart.png -b white -w 2400
```

---

**Catatan:** File flowchart dibuat berdasarkan dokumentasi `alur_proses.txt` dan `profil_aplikasi.md`. Jika ada perubahan alur proses, edit file `.mmd` sesuai kebutuhan.
