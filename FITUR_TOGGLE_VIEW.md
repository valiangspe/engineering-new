# 🔄 Fitur Toggle View - Flowchart & List

## ✨ Update Terbaru

Halaman User Guide kini memiliki **2 mode tampilan** yang dapat di-switch dengan mudah!

---

## 🎯 Fitur Baru

### Toggle Button di Modal Header
- Button toggle terletak di pojok kanan atas modal (sebelah tombol Close)
- Icon berubah sesuai mode:
  - **Flowchart Mode**: Icon `mdi-format-list-bulleted` + text "List View"
  - **List Mode**: Icon `mdi-sitemap` + text "Flowchart View"

### 2 Mode Tampilan

#### 1️⃣ **Flowchart View (Default)**
📊 Tampilan diagram flowchart visual dengan:
- **Shapes berbeda untuk setiap tipe node**:
  - 🟢 **Start/End**: Rounded rectangle (hijau)
  - 🔵 **Process**: Rectangle (biru)
  - 🟠 **Decision**: Diamond shape (orange) - dengan transform rotate 45deg
  - 🔴 **Error**: Rectangle (merah) dengan warning icon
- **Arrows animasi** yang menunjukkan alur
- **Branch labels** untuk decision points (Ya/Tidak)
- **Condition badges** untuk step dengan kondisi
- **Hover effects** pada setiap node
- **Gradient backgrounds** untuk efek 3D

#### 2️⃣ **List View**
📝 Tampilan list detail dengan:
- **Card-based layout** - setiap step dalam card terpisah
- **Step number badge** dengan gradient purple
- **Icon besar** untuk setiap step
- **Type badges** (START/END/PROCESS/DECISION/ERROR)
- **Condition badges** (Ya/Tidak/Valid/Invalid)
- **Branches badges** untuk decision points
- **Color-coded borders**:
  - Hijau: Start/End
  - Biru: Process
  - Orange: Decision
  - Merah: Error
- **Hover effect** - card slide ke kanan saat di-hover

---

## 🚀 Cara Menggunakan

### Step 1: Buka Detail Flowchart
1. Akses halaman `/user-guide`
2. Klik salah satu bagian di flowchart utama (Login, Dashboard, ECN, dll)
3. Modal popup akan muncul dengan **Flowchart View** sebagai default

### Step 2: Switch ke List View
1. Klik tombol **"List View"** di pojok kanan atas modal
2. Tampilan akan berubah ke list detail dengan card

### Step 3: Switch Kembali ke Flowchart
1. Klik tombol **"Flowchart View"**
2. Tampilan kembali ke diagram flowchart visual

### Step 4: Tutup Modal
- Klik tombol **"Tutup"** di bawah
- Atau klik **X** di header
- Atau klik area gelap di luar modal

---

## 💡 Kapan Menggunakan Mode Mana?

### Gunakan **Flowchart View** untuk:
✅ Memahami alur proses secara visual
✅ Melihat decision points dan branching
✅ Presentasi atau demo
✅ Training visual
✅ Quick overview dari mana ke mana

### Gunakan **List View** untuk:
✅ Membaca detail text setiap step
✅ Melihat metadata (type, condition, branches)
✅ Copy-paste deskripsi step
✅ Fokus ke konten text tanpa distraksi visual
✅ Mobile viewing (lebih mudah scroll)

---

## 🎨 Technical Details

### Vue.js Implementation

**Script Setup:**
```javascript
const viewMode = ref('flowchart'); // 'flowchart' atau 'list'

const toggleViewMode = () => {
  viewMode.value = viewMode.value === 'flowchart' ? 'list' : 'flowchart';
};
```

**Template:**
```vue
<!-- Toggle Button -->
<button class="btn-toggle-view" @click="toggleViewMode">
  <i :class="viewMode === 'flowchart' ? 'mdi mdi-format-list-bulleted' : 'mdi mdi-sitemap'"></i>
  {{ viewMode === 'flowchart' ? 'List View' : 'Flowchart View' }}
</button>

<!-- Conditional Rendering -->
<div v-if="viewMode === 'flowchart'" class="modal-body-flowchart">
  <!-- Flowchart diagram with shapes -->
</div>

<div v-else class="modal-body-list">
  <!-- List view with cards -->
</div>
```

### CSS Highlights

**Flowchart Shapes:**
- Start/End: `border-radius: 50px` (pill shape)
- Process: `border-radius: 8px` (rounded rectangle)
- Decision: `transform: rotate(45deg)` (diamond)
- Error: `border-radius: 8px` + red gradient

**List Cards:**
- Border-left color coding per type
- Gradient number badges
- Smooth hover transitions
- Responsive gap spacing

---

## 📊 Comparison

| Feature | Flowchart View | List View |
|---------|---------------|-----------|
| **Visual Appeal** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ |
| **Detail Text** | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Space Efficient** | ⭐⭐⭐ | ⭐⭐⭐⭐ |
| **Easy to Scan** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| **Best for Presentation** | ✅ Yes | ❌ No |
| **Best for Reading** | ❌ No | ✅ Yes |
| **Shows Flow Direction** | ✅ Yes | ⚠️ Partial |
| **Mobile Friendly** | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ |

---

## 🔧 Maintenance

### Menambah Step Baru
Data step sama untuk kedua view, jadi cukup tambahkan di `flowcharts` object:

```javascript
{
  icon: 'mdi-icon-name',
  text: 'Description',
  type: 'start|process|decision|end|error',
  condition: 'Optional condition text',
  branches: ['Option1', 'Option2'] // For decision type
}
```

Kedua view akan otomatis ter-update!

### Mengubah Default View
Edit line ini di `openFlowchart` function:
```javascript
viewMode.value = 'flowchart'; // Ubah ke 'list' jika ingin default list
```

### Mengubah Warna/Style
- **Flowchart**: Edit CSS section `/* ========== FLOWCHART NODES ========== */`
- **List**: Edit CSS section `/* ========== LIST VIEW STYLES ========== */`

---

## 📱 Responsive Design

### Desktop (> 768px)
- Flowchart: Nodes lebar 300-600px
- List: Cards max-width 800px
- Toggle button: Full text visible

### Mobile (< 768px)
- Flowchart: Nodes menyesuaikan (min 280px)
- List: Full width cards
- Toggle button: Icon + text

---

## 🎉 Benefits

### Untuk End User:
✅ **Fleksibilitas**: Bisa pilih cara belajar yang paling cocok
✅ **Accessibility**: Visual learners ↔️ Text readers
✅ **Convenience**: Tidak perlu scroll banyak di flowchart untuk baca detail

### Untuk Trainer:
✅ **Presentation**: Flowchart view untuk demo
✅ **Q&A**: List view untuk jelaskan detail
✅ **Versatility**: Satu halaman, dua fungsi

### Untuk Developer:
✅ **Single source of truth**: Data steps sama untuk kedua view
✅ **Easy maintenance**: Update sekali, semua view ter-update
✅ **Clean code**: Conditional rendering dengan v-if

---

## 🚦 Status

| Feature | Status |
|---------|--------|
| Toggle Button | ✅ Implemented |
| Flowchart View | ✅ Completed |
| List View | ✅ Completed |
| Smooth Transitions | ✅ Working |
| Responsive Design | ✅ Working |
| All 10 Flowcharts | ✅ Supported |

---

## 💬 User Feedback

Silakan berikan feedback untuk improvement:
- Apakah ada mode view lain yang dibutuhkan?
- Layout list view sudah optimal?
- Warna/style perlu adjustment?

---

**Last Updated**: 2025-12-03
**Version**: 2.0
**Author**: Engineering Management System Team
