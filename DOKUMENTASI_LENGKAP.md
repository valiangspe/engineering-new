# 📚 Dokumentasi Lengkap Engineering Management System

Dokumentasi ini berisi semua file dokumentasi yang telah dibuat untuk sistem Engineering Management.

## 📋 Daftar File Dokumentasi

### 1. **profil_aplikasi.md**
**Isi:** Profiling lengkap aplikasi yang mencakup:
- ✅ Ringkasan kegunaan aplikasi
- ✅ Teknologi yang dipakai (Backend & Frontend)
- ✅ Fitur-fitur pada aplikasi
- ✅ Struktur database (21 tables)
- ✅ Struktur project

**Untuk siapa:** Developer, Project Manager, Technical Lead

---

### 2. **alur_proses.txt**
**Isi:** Dokumentasi alur proses untuk setiap bagian aplikasi:
- 1. Login & Autentikasi
- 2. Dashboard - Tampilan Metrics
- 3. Manajemen ECN/CCN (Create, Approval, Viewing)
- 4. BOM Approval Process
- 5. Activity & Task Management
- 6. Support Document Management
- 7. Notification System
- 8. Reporting & Export Data
- 9. Integrasi dengan Sistem Eksternal
- 10. AI Document Analyzer
- 11. Deployment & Environment Management

**Untuk siapa:** Developer, Business Analyst, QA Tester

---

### 3. **flowchart_user_journey.mmd**
**Isi:** Flowchart Mermaid untuk user journey lengkap:
- Alur penggunaan aplikasi dari login sampai logout
- Semua fitur utama (ECN/CCN, Activity, BOM, Support Docs, AI, Reports, Notifications)
- Interaksi antar fitur

**Format:** Mermaid (perlu di-render menjadi gambar)

**Cara render:**
```bash
# Cara 1: Online (termudah)
1. Buka https://mermaid.live
2. Copy isi file flowchart_user_journey.mmd
3. Paste di editor
4. Download PNG/SVG

# Cara 2: Command line
npm install -g @mermaid-js/mermaid-cli
mmdc -i flowchart_user_journey.mmd -o flowchart.png -b white
```

**Untuk siapa:** End User, Trainer, Documentation Team

---

### 4. **CARA_RENDER_FLOWCHART.md**
**Isi:** Panduan lengkap cara render flowchart dari file .mmd:
- 5 metode berbeda (Online, VS Code, CLI, GitHub, Tools)
- Tips kustomisasi warna dan style
- Troubleshooting common issues
- Quick start guide

**Untuk siapa:** Developer, Technical Writer

---

### 5. **UserGuidePage.vue** (Halaman UI)
**Isi:** Halaman web interaktif yang bisa diakses di aplikasi:
- Flowchart visual alur penggunaan (CSS-based)
- 10 panduan fitur utama dalam card format
- Tips & Tricks
- FAQ (Frequently Asked Questions)
- Responsive design

**Akses:** `http://localhost:5173/#/user-guide` (development)

**Untuk siapa:** End User (accessible dari aplikasi)

---

## 🎯 Cara Menggunakan Dokumentasi Ini

### Untuk Developer Baru (Onboarding)
Baca dalam urutan ini:
1. **profil_aplikasi.md** - Pahami teknologi & struktur
2. **alur_proses.txt** - Pahami alur bisnis proses
3. **flowchart_user_journey.mmd** - Visual user journey
4. Explore codebase berdasarkan pemahaman di atas

### Untuk End User / Trainer
1. Akses **UserGuidePage.vue** di aplikasi (`/user-guide`)
2. Atau render **flowchart_user_journey.mmd** untuk presentasi
3. Gunakan FAQ di UserGuidePage untuk troubleshooting

### Untuk Business Analyst / Product Owner
1. **profil_aplikasi.md** - Fitur & capabilities
2. **alur_proses.txt** - Business process flow
3. **flowchart_user_journey.mmd** - User experience flow

### Untuk Technical Writer / Documentation Team
1. Semua file di atas sebagai referensi
2. **CARA_RENDER_FLOWCHART.md** - Cara generate visual
3. Update UserGuidePage.vue jika ada perubahan fitur

---

## 📊 Struktur File Dokumentasi

```
engineering-new/
├── profil_aplikasi.md              # Profil aplikasi lengkap
├── alur_proses.txt                 # Alur proses setiap bagian
├── flowchart_user_journey.mmd      # Flowchart Mermaid user journey
├── CARA_RENDER_FLOWCHART.md        # Panduan render flowchart
├── DOKUMENTASI_LENGKAP.md          # File ini (index dokumentasi)
└── frontend/
    └── src/
        └── pages/
            └── UserGuidePage.vue   # Halaman panduan di aplikasi
```

---

## 🚀 Quick Access

| Dokumentasi | Path | Format | Status |
|-------------|------|--------|--------|
| Profil Aplikasi | `profil_aplikasi.md` | Markdown | ✅ Complete |
| Alur Proses | `alur_proses.txt` | Text | ✅ Complete |
| Flowchart User Journey | `flowchart_user_journey.mmd` | Mermaid | ✅ Complete |
| Cara Render Flowchart | `CARA_RENDER_FLOWCHART.md` | Markdown | ✅ Complete |
| User Guide Page | `frontend/src/pages/UserGuidePage.vue` | Vue.js | ✅ Complete |

---

## 🔄 Update Documentation

Jika ada perubahan pada aplikasi, update dokumentasi dalam urutan:

1. **profil_aplikasi.md** - Update fitur/teknologi baru
2. **alur_proses.txt** - Update alur proses yang berubah
3. **flowchart_user_journey.mmd** - Update flowchart jika ada fitur baru
4. **UserGuidePage.vue** - Update panduan UI untuk end user

---

## 📝 Catatan Penting

### Teknologi Stack
- **Backend:** ASP.NET Core (.NET 8), MySQL, Entity Framework Core
- **Frontend:** Vue.js 3, Vite, Vuetify 3, Bootstrap 5
- **Tools:** Mermaid untuk flowchart, ClosedXML untuk Excel export

### Database Tables (21 tables)
- Core: EngineeringDetailProblems, EngineeringActivities, Tasks, InCharges
- BOM: BomApprovals, BomApprovalPics
- Support: SupportTables, SupportEngineeringDocuments
- Analytics: DashboardMetrics, ECNData, CCNData
- Config: Configurations, EngDeptConfigs, PanelProcesses
- Notifications: Notifications, UserRoles
- AI: InquiryAIGenerations
- Others: EngineerSupports, SupportDetails, OutstandingPostPOs, DonePOs

### Integrasi Eksternal
- **Auth Server:** authserver-backend.iotech.my.id
- **CRM:** crm-local.iotech.my.id
- **PPIC:** ppic-backend.iotech.my.id

---

## 💡 Tips

1. **Untuk Presentasi:** Render flowchart_user_journey.mmd ke PNG dengan ukuran besar
   ```bash
   mmdc -i flowchart_user_journey.mmd -o flowchart.png -w 2400 -b white
   ```

2. **Untuk Dokumentasi Online:** Push ke GitHub, flowchart Mermaid akan otomatis ter-render

3. **Untuk Training:** Gunakan UserGuidePage.vue langsung di aplikasi (live demo)

4. **Untuk Development:** Baca profil_aplikasi.md + alur_proses.txt terlebih dahulu

---

## 📞 Support

Untuk pertanyaan lebih lanjut mengenai dokumentasi:
- Engineering Department
- IT Support Team

---

**Last Updated:** 2025-12-02
**Version:** 1.0
**Maintained by:** Engineering Management System Team
