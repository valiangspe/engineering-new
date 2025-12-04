# 📖 Fitur User Guide Interaktif - Engineering Management System

## ✨ Overview

Halaman **User Guide** yang baru telah ditambahkan ke aplikasi dengan fitur flowchart interaktif yang dapat diklik untuk melihat alur detail setiap proses!

---

## 🎯 Fitur Utama

### 1. **Flowchart Utama (Main User Journey)**
- Flowchart visual yang menampilkan alur penggunaan aplikasi dari Login → Dashboard → Fitur → Selesai
- **10 Section clickable** yang dapat diklik untuk melihat detail

### 2. **Flowchart Detail (10 Modal Popup)**
Setiap bagian memiliki flowchart detail yang muncul dalam modal popup:

| No | Bagian | Jumlah Steps | Warna | Fitur Khusus |
|----|--------|--------------|-------|--------------|
| 1 | 🔐 Login & Autentikasi | 10 steps | Cyan | Decision flow (Valid/Tidak) |
| 2 | 📊 Dashboard | 8 steps | Purple | API endpoints detail |
| 3 | 📝 ECN/CCN Management | 9 steps | Blue | Upload attachment flow |
| 4 | ⚙️ Activity & Task Creation | 11 steps | Orange | Multiple task creation |
| 5 | ✅ Task Execution Flow | 21 steps | Green | PIC → SPV → Manager flow |
| 6 | ✅ BOM Approval Process | 14 steps | Teal | Multiple PICs approval |
| 7 | 📁 Support Document Management | 14 steps | Pink | 4 action branches |
| 8 | 🤖 AI Document Analyzer | 15 steps | Deep Purple | AI processing flow |
| 9 | 📈 Reports & Export | 11 steps | Red-Orange | Excel export flow |
| 10 | 🔔 Notification System | 14 steps | Light Blue | Undo functionality |

**Total: 127 detailed steps!** 🎉

---

## 🚀 Cara Mengakses

### Method 1: Melalui URL
```
http://localhost:5173/#/user-guide
```
atau
```
http://your-domain.com/#/user-guide
```

### Method 2: Melalui Header
Klik tombol **"User Guide"** (📖) di header/navbar (kanan atas)

### Method 3: Melalui Sidebar Menu
1. Klik menu hamburger (☰)
2. Pilih **"📖 User Guide"** (posisi ke-2 setelah Dashboard)

---

## 🎨 Cara Menggunakan Flowchart Interaktif

### Step 1: Lihat Flowchart Utama
Scroll ke section "**Alur Penggunaan Aplikasi**" di halaman User Guide

### Step 2: Klik Bagian yang Ingin Dipelajari
Pilih salah satu dari 10 bagian:
- 🔐 Login
- 📊 Dashboard
- 📝 ECN/CCN
- ⚙️ Activity/Task
- ✅ Task Execution
- ✅ BOM Approval
- 📁 Support Docs
- 🤖 AI Analyzer
- 📈 Reports
- 🔔 Notifications

### Step 3: Lihat Detail di Modal Popup
- Modal akan muncul dengan flowchart detail
- Setiap step memiliki:
  - **Nomor urut**
  - **Icon** yang relevan
  - **Deskripsi lengkap**
  - **Warna coding** berdasarkan tipe (start, process, decision, end, error)

### Step 4: Tutup Modal
- Klik tombol **"Tutup"** di bawah
- Atau klik tombol **X** di pojok kanan atas
- Atau klik area gelap di luar modal

---

## 🎨 Visual Features

### Warna Coding (Step Types):
- 🟢 **Start/End**: Hijau (Success)
- 🔵 **Process**: Biru (Normal process)
- 🟠 **Decision**: Orange (Pilihan/branching)
- 🔴 **Error**: Merah (Error handling)

### Conditional Highlighting:
- **Border kiri hijau**: Step untuk kondisi "Ya"
- **Border kiri merah**: Step untuk kondisi "Tidak"
- **Border kiri ungu**: Step untuk actions (Upload/Link/Download)
- **Border kiri kuning**: Step untuk output (View/Excel)

### Interaktif:
- **Hover effect**: Kotak bergeser sedikit saat di-hover
- **Click hint**: Muncul hint "Klik untuk detail" saat hover
- **Smooth animation**: Fade in/slide up animation untuk modal

---

## 📊 Statistik Dokumentasi

| Metrik | Jumlah |
|--------|--------|
| Total Halaman | 1 |
| Total Flowchart | 11 (1 main + 10 detail) |
| Total Steps | 127+ |
| Total Icons | 127+ (Material Design Icons) |
| Total API Endpoints Documented | 30+ |
| Total Decision Points | 15+ |
| Bahasa | Indonesia |

---

## 💡 Tips Penggunaan

### Untuk End User:
1. **Mulai dari Login**: Pelajari flow Login terlebih dahulu
2. **Dashboard dulu**: Pahami apa saja yang ada di Dashboard
3. **Fokus ke fitur yang sering dipakai**: ECN/CCN, Activity, Notifications
4. **Gunakan sebagai reference**: Saat lupa langkah-langkah tertentu

### Untuk Trainer:
1. **Demo live**: Tampilkan di projector saat training
2. **Print flowchart**: Klik modal, screenshot, print untuk handout
3. **Step-by-step**: Ikuti flowchart untuk demo yang terstruktur
4. **Interactive session**: Biarkan trainee klik sendiri untuk eksplorasi

### Untuk Developer:
1. **Dokumentasi teknis**: Lihat API endpoints yang digunakan
2. **Flow validation**: Pastikan flow sesuai dengan implementasi
3. **Onboarding**: Reference untuk developer baru
4. **Testing guide**: Gunakan flowchart untuk test case

---

## 🔧 Technical Details

### Technology Stack:
- **Framework**: Vue.js 3 (Composition API)
- **Icons**: Material Design Icons (@mdi/font)
- **Styling**: Pure CSS (No external flowchart library)
- **Animation**: CSS Keyframes
- **Responsive**: Mobile-friendly

### Files Modified:
1. `frontend/src/pages/UserGuidePage.vue` (Main file)
2. `frontend/src/main.js` (Route added)
3. `frontend/src/Navbar.vue` (Menu items added)

### Code Structure:
```javascript
// Data structure for each flowchart
const flowcharts = {
  'flowchart-name': {
    title: 'Title',
    color: '#HEX',
    steps: [
      {
        icon: 'mdi-icon-name',
        text: 'Description',
        type: 'start|process|decision|end|error',
        condition: 'Ya|Tidak|...',  // optional
        branches: ['Option1', 'Option2']  // for decision type
      }
    ]
  }
}
```

---

## 🎯 Benefits

### Untuk User:
✅ **Mudah dipahami**: Visual flowchart lebih mudah daripada text
✅ **Self-service**: User bisa belajar sendiri tanpa tanya-tanya
✅ **Always accessible**: Bisa diakses kapan saja dari aplikasi
✅ **Step-by-step**: Tidak akan lupa langkah-langkah

### Untuk Tim:
✅ **Reduce support tickets**: User bisa cek sendiri
✅ **Onboarding faster**: New user lebih cepat belajar
✅ **Consistent training**: Semua user dapat informasi yang sama
✅ **Living documentation**: Bisa di-update kapan saja

### Untuk Perusahaan:
✅ **Increase adoption**: User lebih berani pakai fitur baru
✅ **Reduce training cost**: Less training sessions needed
✅ **Better UX**: User experience yang lebih baik
✅ **Professional image**: Aplikasi terlihat lebih professional

---

## 🔄 Maintenance

### Update Flowchart:
1. Edit file `UserGuidePage.vue`
2. Temukan section `flowcharts` di `<script setup>`
3. Update `steps` array untuk flowchart yang ingin diubah
4. Save dan refresh browser

### Tambah Flowchart Baru:
1. Tambah entry baru di object `flowcharts`
2. Tambah branch item baru di main flowchart
3. Tambah `@click="openFlowchart('new-name')"`

### Ubah Warna/Style:
Edit section `<style scoped>` di bagian bawah file

---

## 📞 Support

Jika ada pertanyaan atau ingin menambah fitur:
- Engineering Department
- IT Support
- Developer Team

---

## 🎉 Kesimpulan

Fitur User Guide Interaktif ini memberikan dokumentasi yang **lengkap**, **visual**, dan **mudah diakses** untuk semua user Engineering Management System. Dengan 10 flowchart detail yang dapat diklik, user dapat mempelajari setiap proses dengan mudah tanpa perlu membaca dokumentasi text yang panjang!

**Happy Learning! 📖✨**

---

**Last Updated**: 2025-12-03
**Version**: 1.0
**Author**: Engineering Management System Team
