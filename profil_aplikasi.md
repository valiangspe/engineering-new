# Profiling Aplikasi Engineering Management System

## 1. Ringkasan Kegunaan Aplikasi
Aplikasi ini adalah sistem manajemen rekayasa (Engineering Management System) yang dirancang khusus untuk departemen teknik di suatu perusahaan manufaktur. Fungsinya adalah untuk mengelola seluruh kegiatan rekayasa, termasuk penanganan Engineering Change Notices (ECN), Change Control Notices (CCN), approval Bill of Materials (BOM), tracking manpower, manajemen task, dan dukungan dokumentasi teknik. Sistem ini terintegrasi dengan sistem lain seperti CRM, PPIC, dan sistem autentikasi untuk memberikan pandangan holistik terhadap proses rekayasa dan memastikan efisiensi dalam produksi dan pengembangan produk.

## 2. Teknologi yang Digunakan
### Backend
- **Framework**: ASP.NET Core (.NET 8) dengan Minimal APIs
- **Database**: MySQL (dengan Entity Framework Core sebagai ORM)
- **Bahasa Pemrograman**: C#
- **Lainnya**: ClosedXML untuk ekspor Excel, CORS untuk cross-origin requests

### Frontend
- **Framework**: Vue.js 3 (dengan Composition API dan `<script setup>`)
- **Build Tool**: Vite
- **UI Framework**: Vuetify 3 dan Bootstrap 5
- **State Management**: Pinia (Vue Store)
- **Routing**: Vue Router 4
- **Bahasa Pemrograman**: JavaScript
- **Library Tambahan**:
  - Axios untuk HTTP requests
  - Chart.js & vue-chartjs untuk visualisasi data
  - JWT-decode untuk autentikasi
  - Lodash untuk utility functions
  - ExcelJS untuk ekspor Excel
  - Chroma-js untuk manipulasi warna
  - UUID untuk generate unique identifiers

## 3. Fitur pada Aplikasi
- **Dashboard**: Menampilkan metrik utama seperti jumlah manpower, jumlah ECN/CCN bulan ini, total produk/BOM, dll.
- **Manajemen ECN/CCN**: Pembuatan, approval, dan tracking Engineering Change Notices dan Change Control Notices
- **BOM Approval**: Proses approval untuk Bill of Materials dengan multiple PIC (Person In Charge)
- **Activity Management**: Manajemen aktivitas rekayasa dengan tugas-tugas, in-charge, dan tracking waktu
- **Manpower Tracking**: Monitoring dan reporting tenaga kerja departemen teknik
- **Support Document Management**: Unggah dan manajemen dokumen pendukung teknis
- **Notification System**: Pemberitahuan tugas antar PIC, Supervisor, dan Manager
- **AI Document Analyzer**: Analisis dokumen menggunakan kecerdasan buatan (Artificial Intelligence) untuk pemrosesan inquiry dan dokumen teknis
- **Reporting**: Ekspor data ke Excel, laporan ECN/CCN, dan laporan produktivitas
- **Authentication & Authorization**: Integrasi dengan sistem autentikasi eksternal dan role-based access

## 4. Struktur Database
Database menggunakan MySQL dengan Entitas-Entity berikut (dikelompokkan berdasarkan fungsi):

### Core Engineering Management
- **EngineeringDetailProblems**: Menyimpan data utama ECN/CCN (ID, proyek, deskripsi, status, approval info, dll.)
- **EngineeringDetailProblemItems**: Item-item yang terkait dengan ECN/CCN (part number, quantity, harga, tipe increase/decrease)
- **EngineeringDetailProblemApprovals**: History approval untuk setiap ECN/CCN
- **EngineeringActivities**: Aktivitas rekayasa dengan tipe (PrePO, PostPO, Others), linked ke inquiry/PO/ECN
- **Tasks**: Sub-task dalam setiap aktivitas dengan tracking waktu, status completion (PIC, SPV, Manager), dan due dates
- **InCharges**: Orang-orang yang bertanggung jawab pada setiap task (multiple users per task)

### BOM Management
- **BomApprovals**: Rekaman approval BOM
- **BomApprovalPics**: PIC-PIC yang terlibat dalam approval BOM

### Support & Documentation
- **SupportTables**: Tabel Dokumen pendukung yang dapat diunggah
- **SupportEngineeringDocuments**: Linking antara job dan dokumen pendukung
- **SupportDetails**: Detail dukungan teknis
- **InquiryAIGenerations**: Data hasil analisis AI terhadap dokumen inquiry

### Reporting & Analytics
- **DashboardMetrics**: Data agregat untuk dashboard
- **ECNData**: Data historis ECN per bulan (untuk charting)
- **CCNData**: Data historis CCN per bulan

### User Management
- **UserRoles**: Peran pengguna dan hubungannya
- **Notifications**: Sistem notifikasi untuk task flow

### Configuration
- **Configurations**: Pengaturan umum sistem
- **EngDeptConfigs**: Konfigurasi khusus departemen teknik
- **PanelProcesses**: Proses panel untuk kalkulasi waktu

### Reporting
- **OutstandingPostPOs**: Tracking outstanding purchase orders
- **DonePOs**: History PO yang sudah selesai

## 5. Struktur Project
```
engineering-new/
├── backend/                    # Direktori backend .NET
│   ├── Controllers/            # (Jika ada, tapi API menggunakan Minimal APIs di Program.cs)
│   ├── Migrations/            # Database migrations Entity Framework
│   ├── Models/                # Definisi model data (entitas)
│   ├── Helpers/               # Helper classes (FileHelper, dll.)
│   ├── files/                 # Penyimpanan file upload ECN
│   ├── uploads/               # Penyimpanan file support documents
│   ├── Notifications/         # Konfigurasi notifikasi
│   ├── Properties/            # Konfigurasi VS
│   ├── Program.cs             # Main entry point dan API endpoints
│   ├── backend.csproj         # File projek .NET
│   ├── appsettings.json       # Konfigurasi aplikasi
│   └── README                 # Dokumentasi backend
├── frontend/                  # Direktori frontend Vue.js
│   ├── public/                # Static assets
│   ├── src/                   # Source code
│   │   ├── assets/            # Gambar dan assets lain
│   │   ├── components/        # Komponen Vue reusable
│   │   ├── pages/             # Pages/routes aplikasi
│   │   ├── helpers.js         # Helper functions
│   │   ├── main.js           # Entry point aplikasi
│   │   ├── App.vue           # Komponen root
│   │   └── styles/            # CSS styles
│   ├── package.json          # Dependencies Node.js
│   ├── vite.config.js        # Konfigurasi Vite
│   └── index.html            # Template HTML utama
├── manage.py                  # Python script untuk manage environment
├── Dockerfile                 # Containerization konfigurasi
└── README.md                  # Dokumentasi umum proyek
