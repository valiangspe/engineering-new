<template>
  <div class="user-guide-page">
    <div class="container-fluid py-4">
      <!-- Header -->
      <div class="row mb-4">
        <div class="col-12">
          <div class="card shadow-sm">
            <div class="card-body">
              <h2 class="mb-3">
                <i class="mdi mdi-book-open-page-variant"></i>
                Panduan Penggunaan Aplikasi Engineering Management System
              </h2>
              <p class="text-muted mb-0">
                Panduan lengkap alur penggunaan aplikasi untuk semua user
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Flowchart Simplified -->
      <div class="row mb-4">
        <div class="col-12">
          <div class="card shadow-sm">
            <div class="card-header bg-primary text-white">
              <h5 class="mb-0">
                <i class="mdi mdi-sitemap"></i>
                Alur Penggunaan Aplikasi
              </h5>
            </div>
            <div class="card-body">
              <div class="flow-diagram">
                <div class="flow-step clickable" @click="openFlowchart('login')">
                  <div class="step-icon bg-info">
                    <i class="mdi mdi-login"></i>
                  </div>
                  <div class="step-content">
                    <h6>1. LOGIN</h6>
                    <p>Masukkan username & password untuk autentikasi</p>
                  </div>
                  <div class="click-hint">
                    <i class="mdi mdi-cursor-pointer"></i> Klik untuk detail
                  </div>
                </div>
                <div class="flow-arrow">⬇</div>

                <div class="flow-step clickable" @click="openFlowchart('dashboard')">
                  <div class="step-icon bg-purple">
                    <i class="mdi mdi-view-dashboard"></i>
                  </div>
                  <div class="step-content">
                    <h6>2. DASHBOARD</h6>
                    <p>Lihat metrics utama dan pilih menu fitur</p>
                  </div>
                  <div class="click-hint">
                    <i class="mdi mdi-cursor-pointer"></i> Klik untuk detail
                  </div>
                </div>
                <div class="flow-arrow">⬇</div>

                <div class="flow-branches">
                  <div class="branch">
                    <div class="branch-item" @click="openFlowchart('ecn')">
                      <i class="mdi mdi-file-document-edit text-primary"></i>
                      <span>ECN/CCN</span>
                      <small class="click-hint-small">Klik untuk detail</small>
                    </div>
                    <div class="branch-item" @click="openFlowchart('activity')">
                      <i class="mdi mdi-clipboard-check text-warning"></i>
                      <span>Activity/Task</span>
                      <small class="click-hint-small">Klik untuk detail</small>
                    </div>
                    <div class="branch-item" @click="openFlowchart('task-execution')">
                      <i class="mdi mdi-progress-check text-success"></i>
                      <span>Task Execution</span>
                      <small class="click-hint-small">Klik untuk detail</small>
                    </div>
                    <div class="branch-item" @click="openFlowchart('bom')">
                      <i class="mdi mdi-file-check text-teal"></i>
                      <span>BOM Approval</span>
                      <small class="click-hint-small">Klik untuk detail</small>
                    </div>
                    <div class="branch-item" @click="openFlowchart('support')">
                      <i class="mdi mdi-folder-upload text-danger"></i>
                      <span>Support Docs</span>
                      <small class="click-hint-small">Klik untuk detail</small>
                    </div>
                    <div class="branch-item" @click="openFlowchart('ai')">
                      <i class="mdi mdi-robot text-purple"></i>
                      <span>AI Analyzer</span>
                      <small class="click-hint-small">Klik untuk detail</small>
                    </div>
                    <div class="branch-item" @click="openFlowchart('reports')">
                      <i class="mdi mdi-chart-bar text-orange"></i>
                      <span>Reports</span>
                      <small class="click-hint-small">Klik untuk detail</small>
                    </div>
                    <div class="branch-item" @click="openFlowchart('notifications')">
                      <i class="mdi mdi-bell text-info"></i>
                      <span>Notifications</span>
                      <small class="click-hint-small">Klik untuk detail</small>
                    </div>
                  </div>
                </div>
                <div class="flow-arrow">⬇</div>

                <div class="flow-step">
                  <div class="step-icon bg-success">
                    <i class="mdi mdi-check-circle"></i>
                  </div>
                  <div class="step-content">
                    <h6>3. SELESAI</h6>
                    <p>Task completed atau logout dari aplikasi</p>
                  </div>
                </div>
              </div>
              <div class="alert alert-success mt-4">
                <i class="mdi mdi-information-outline"></i>
                <strong>Info:</strong> Klik pada setiap kotak di flowchart untuk melihat alur detail!
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Guide Cards -->
      <div class="row">
        <div class="col-12">
          <h4 class="mb-3">
            <i class="mdi mdi-lightning-bolt"></i>
            Panduan Cepat Fitur Utama
          </h4>
        </div>

        <!-- Login Card -->
        <div class="col-md-6 col-lg-4 mb-4">
          <div class="card h-100 shadow-sm guide-card">
            <div class="card-header bg-info text-white">
              <h5 class="mb-0">
                <i class="mdi mdi-login"></i>
                1. Login & Autentikasi
              </h5>
            </div>
            <div class="card-body">
              <ol>
                <li>Buka aplikasi Engineering Management</li>
                <li>Masukkan <strong>Username</strong> dan <strong>Password</strong></li>
                <li>Klik tombol <strong>Login</strong></li>
                <li>Jika berhasil, akan diarahkan ke Dashboard</li>
              </ol>
              <div class="alert alert-info">
                <i class="mdi mdi-information"></i>
                <strong>Tips:</strong> Token disimpan otomatis untuk session berikutnya
              </div>
            </div>
          </div>
        </div>

        <!-- Dashboard Card -->
        <div class="col-md-6 col-lg-4 mb-4">
          <div class="card h-100 shadow-sm guide-card">
            <div class="card-header bg-purple text-white">
              <h5 class="mb-0">
                <i class="mdi mdi-view-dashboard"></i>
                2. Dashboard
              </h5>
            </div>
            <div class="card-body">
              <p><strong>Anda bisa melihat:</strong></p>
              <ul>
                <li>Total ECN/CCN bulan ini</li>
                <li>Total Manpower</li>
                <li>Total BOM yang perlu approval</li>
                <li>Chart ECN/CCN per bulan</li>
                <li>Aktivitas terbaru</li>
              </ul>
              <div class="alert alert-success">
                <i class="mdi mdi-chart-line"></i>
                Dashboard update secara real-time
              </div>
            </div>
          </div>
        </div>

        <!-- ECN/CCN Card -->
        <div class="col-md-6 col-lg-4 mb-4">
          <div class="card h-100 shadow-sm guide-card">
            <div class="card-header bg-primary text-white">
              <h5 class="mb-0">
                <i class="mdi mdi-file-document-edit"></i>
                3. ECN/CCN Management
              </h5>
            </div>
            <div class="card-body">
              <p><strong>Membuat ECN/CCN:</strong></p>
              <ol>
                <li>Pilih menu <strong>ECN/CCN</strong></li>
                <li>Klik <strong>Buat Baru</strong></li>
                <li>Isi form:
                  <ul>
                    <li>Project Name</li>
                    <li>Detail Problem</li>
                    <li>Engineering Notes</li>
                    <li>Type (ECN/CCN)</li>
                    <li>Items (Part Number, Qty, Price)</li>
                  </ul>
                </li>
                <li>Upload attachment jika ada</li>
                <li>Klik <strong>Submit</strong></li>
              </ol>
            </div>
          </div>
        </div>

        <!-- Activity/Task Card -->
        <div class="col-md-6 col-lg-4 mb-4">
          <div class="card h-100 shadow-sm guide-card">
            <div class="card-header bg-warning text-dark">
              <h5 class="mb-0">
                <i class="mdi mdi-clipboard-check"></i>
                4. Activity & Task
              </h5>
            </div>
            <div class="card-body">
              <p><strong>Membuat Work Order:</strong></p>
              <ol>
                <li>Pilih menu <strong>WO (Work Order)</strong></li>
                <li>Pilih Type: PrePO, PostPO, atau Others</li>
                <li>Link ke PO/Inquiry/ECN (opsional)</li>
                <li>Tambah Tasks:
                  <ul>
                    <li>Description</li>
                    <li>Assign PIC (Person In Charge)</li>
                    <li>Estimasi Hours</li>
                    <li>Due Date</li>
                  </ul>
                </li>
                <li>Submit Activity</li>
              </ol>
              <div class="alert alert-warning">
                <i class="mdi mdi-bell-ring"></i>
                PIC akan mendapat notifikasi otomatis
              </div>
            </div>
          </div>
        </div>

        <!-- Task Execution Card -->
        <div class="col-md-6 col-lg-4 mb-4">
          <div class="card h-100 shadow-sm guide-card">
            <div class="card-header bg-success text-white">
              <h5 class="mb-0">
                <i class="mdi mdi-progress-check"></i>
                5. Task Execution
              </h5>
            </div>
            <div class="card-body">
              <p><strong>Alur Pengerjaan Task:</strong></p>
              <ol>
                <li><strong>PIC</strong> menerima notifikasi</li>
                <li>PIC kerjakan task</li>
                <li>PIC mark <strong>Done</strong> ✓</li>
                <li>Notifikasi otomatis ke <strong>SPV</strong></li>
                <li>SPV review dan mark <strong>Done</strong> ✓</li>
                <li>Notifikasi ke <strong>Manager</strong></li>
                <li>Manager approve dan mark <strong>Done</strong> ✓</li>
                <li>Task completed! 🎉</li>
              </ol>
              <div class="alert alert-info">
                <i class="mdi mdi-undo"></i>
                Bisa <strong>Undo</strong> jika ada kesalahan
              </div>
            </div>
          </div>
        </div>

        <!-- BOM Approval Card -->
        <div class="col-md-6 col-lg-4 mb-4">
          <div class="card h-100 shadow-sm guide-card">
            <div class="card-header bg-teal text-white">
              <h5 class="mb-0">
                <i class="mdi mdi-file-check"></i>
                6. BOM Approval
              </h5>
            </div>
            <div class="card-body">
              <p><strong>Proses Approval BOM:</strong></p>
              <ol>
                <li>Pilih menu <strong>BOM Approval</strong></li>
                <li>Lihat list BOM yang perlu approval</li>
                <li>Klik untuk review details</li>
                <li>Approve/Reject dengan remark</li>
                <li>Jika semua PIC approve → BOM snapshot dikirim ke PPIC</li>
              </ol>
              <div class="alert alert-warning">
                <i class="mdi mdi-account-multiple"></i>
                BOM memerlukan approval dari <strong>multiple PICs</strong>
              </div>
            </div>
          </div>
        </div>

        <!-- Support Document Card -->
        <div class="col-md-6 col-lg-4 mb-4">
          <div class="card h-100 shadow-sm guide-card">
            <div class="card-header bg-pink text-white">
              <h5 class="mb-0">
                <i class="mdi mdi-folder-upload"></i>
                7. Support Documents
              </h5>
            </div>
            <div class="card-body">
              <p><strong>Mengelola Dokumen:</strong></p>
              <ul>
                <li><strong>Upload:</strong> Upload dokumen baru</li>
                <li><strong>Link to Job:</strong> Hubungkan dokumen ke job tertentu</li>
                <li><strong>Download:</strong> Download dokumen yang sudah ada</li>
                <li><strong>View Job Docs:</strong> Lihat semua dokumen untuk job tertentu</li>
              </ul>
              <div class="alert alert-info">
                <i class="mdi mdi-file-multiple"></i>
                Support multiple file formats
              </div>
            </div>
          </div>
        </div>

        <!-- AI Analyzer Card -->
        <div class="col-md-6 col-lg-4 mb-4">
          <div class="card h-100 shadow-sm guide-card">
            <div class="card-header bg-deep-purple text-white">
              <h5 class="mb-0">
                <i class="mdi mdi-robot"></i>
                8. AI Document Analyzer
              </h5>
            </div>
            <div class="card-body">
              <p><strong>Menggunakan AI Analyzer:</strong></p>
              <ol>
                <li>Pilih menu <strong>AI Analyzer</strong></li>
                <li>Pilih input type:
                  <ul>
                    <li>Upload Document</li>
                    <li>Pilih Inquiry dari list</li>
                  </ul>
                </li>
                <li>Klik <strong>Analyze</strong></li>
                <li>AI akan memproses dan generate analisis</li>
                <li>Hasil ditampilkan dalam format Markdown</li>
                <li>Copy hasil ke Engineering Notes atau Task Description</li>
              </ol>
              <div class="alert alert-success">
                <i class="mdi mdi-magic-staff"></i>
                AI membantu mempercepat analisis dokumen teknis
              </div>
            </div>
          </div>
        </div>

        <!-- Reports Card -->
        <div class="col-md-6 col-lg-4 mb-4">
          <div class="card h-100 shadow-sm guide-card">
            <div class="card-header bg-orange text-white">
              <h5 class="mb-0">
                <i class="mdi mdi-chart-bar"></i>
                9. Reports
              </h5>
            </div>
            <div class="card-body">
              <p><strong>Generate Reports:</strong></p>
              <ol>
                <li>Pilih menu <strong>Reports</strong></li>
                <li>Pilih jenis report:
                  <ul>
                    <li>ECN/CCN Report</li>
                    <li>Activity Report</li>
                    <li>Manpower Report</li>
                  </ul>
                </li>
                <li>Set filter (Date Range, User, Inquiry, dll.)</li>
                <li>View hasil di browser atau</li>
                <li>Klik <strong>Export to Excel</strong> untuk download</li>
              </ol>
              <div class="alert alert-info">
                <i class="mdi mdi-microsoft-excel"></i>
                Export langsung ke Excel format
              </div>
            </div>
          </div>
        </div>

        <!-- Notifications Card -->
        <div class="col-md-6 col-lg-4 mb-4">
          <div class="card h-100 shadow-sm guide-card">
            <div class="card-header bg-light-blue text-white">
              <h5 class="mb-0">
                <i class="mdi mdi-bell"></i>
                10. Notifications
              </h5>
            </div>
            <div class="card-body">
              <p><strong>Menggunakan Notifikasi:</strong></p>
              <ul>
                <li>Klik icon <i class="mdi mdi-bell"></i> di navbar</li>
                <li>Lihat task yang assigned ke Anda</li>
                <li>Klik notifikasi untuk detail</li>
                <li>Kerjakan task sesuai role Anda (PIC/SPV/Manager)</li>
                <li>Mark as Done setelah selesai</li>
              </ul>
              <div class="alert alert-success">
                <i class="mdi mdi-check-circle"></i>
                Notifikasi real-time untuk workflow yang efisien
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Tips & Tricks -->
      <div class="row mt-4">
        <div class="col-12">
          <div class="card shadow-sm">
            <div class="card-header bg-dark text-white">
              <h5 class="mb-0">
                <i class="mdi mdi-lightbulb-on"></i>
                Tips & Tricks
              </h5>
            </div>
            <div class="card-body">
              <div class="row">
                <div class="col-md-6">
                  <h6><i class="mdi mdi-star text-warning"></i> Tips Umum:</h6>
                  <ul>
                    <li>Selalu isi <strong>Engineering Notes</strong> dengan detail untuk referensi tim</li>
                    <li>Gunakan <strong>AI Analyzer</strong> untuk mempercepat analisis inquiry</li>
                    <li>Cek <strong>Notifications</strong> secara berkala untuk task terbaru</li>
                    <li>Gunakan <strong>Filter Date</strong> di reports untuk analisis periode tertentu</li>
                  </ul>
                </div>
                <div class="col-md-6">
                  <h6><i class="mdi mdi-alert-circle text-danger"></i> Hal yang Perlu Diperhatikan:</h6>
                  <ul>
                    <li>ECN/CCN harus di-<strong>approve</strong> oleh manager sebelum diproses</li>
                    <li>Task flow: <strong>PIC → SPV → Manager</strong> (harus berurutan)</li>
                    <li>BOM approval memerlukan persetujuan <strong>semua PICs</strong></li>
                    <li>Support documents harus di-<strong>link ke Job</strong> untuk tracking</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- FAQ -->
      <div class="row mt-4">
        <div class="col-12">
          <div class="card shadow-sm">
            <div class="card-header bg-secondary text-white">
              <h5 class="mb-0">
                <i class="mdi mdi-frequently-asked-questions"></i>
                Frequently Asked Questions (FAQ)
              </h5>
            </div>
            <div class="card-body">
              <div class="accordion" id="faqAccordion">
                <!-- FAQ 1 -->
                <div class="accordion-item">
                  <h2 class="accordion-header">
                    <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#faq1">
                      <strong>Q:</strong> Bagaimana cara membuat ECN baru?
                    </button>
                  </h2>
                  <div id="faq1" class="accordion-collapse collapse show" data-bs-parent="#faqAccordion">
                    <div class="accordion-body">
                      <strong>A:</strong> Masuk ke menu ECN/CCN → Klik "Buat Baru" → Isi form (Project Name, Detail Problem, Engineering Notes, Items) → Upload attachment jika ada → Submit.
                    </div>
                  </div>
                </div>

                <!-- FAQ 2 -->
                <div class="accordion-item">
                  <h2 class="accordion-header">
                    <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faq2">
                      <strong>Q:</strong> Saya sudah mark task "Done" tapi belum complete?
                    </button>
                  </h2>
                  <div id="faq2" class="accordion-collapse collapse" data-bs-parent="#faqAccordion">
                    <div class="accordion-body">
                      <strong>A:</strong> Task memiliki 3 tahap approval (PIC → SPV → Manager). Task baru fully completed setelah Manager mark done. Cek current status di notification.
                    </div>
                  </div>
                </div>

                <!-- FAQ 3 -->
                <div class="accordion-item">
                  <h2 class="accordion-header">
                    <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faq3">
                      <strong>Q:</strong> Bagaimana cara export report ke Excel?
                    </button>
                  </h2>
                  <div id="faq3" class="accordion-collapse collapse" data-bs-parent="#faqAccordion">
                    <div class="accordion-body">
                      <strong>A:</strong> Masuk ke menu Reports → Pilih jenis report → Set filter → Klik tombol "Export to Excel" → File akan otomatis terdownload.
                    </div>
                  </div>
                </div>

                <!-- FAQ 4 -->
                <div class="accordion-item">
                  <h2 class="accordion-header">
                    <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faq4">
                      <strong>Q:</strong> Apa fungsi AI Document Analyzer?
                    </button>
                  </h2>
                  <div id="faq4" class="accordion-collapse collapse" data-bs-parent="#faqAccordion">
                    <div class="accordion-body">
                      <strong>A:</strong> AI Analyzer membantu menganalisis dokumen teknis atau inquiry secara otomatis menggunakan AI. Hasil analisis bisa langsung di-copy ke Engineering Notes untuk mempercepat proses.
                    </div>
                  </div>
                </div>

                <!-- FAQ 5 -->
                <div class="accordion-item">
                  <h2 class="accordion-header">
                    <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#faq5">
                      <strong>Q:</strong> Bagaimana cara undo task yang sudah di-mark done?
                    </button>
                  </h2>
                  <div id="faq5" class="accordion-collapse collapse" data-bs-parent="#faqAccordion">
                    <div class="accordion-body">
                      <strong>A:</strong> Klik tombol "Undo" atau "Undone" pada task. Status akan rollback ke tahap sebelumnya. Hanya bisa dilakukan jika belum fully completed.
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Footer Info -->
      <div class="row mt-4">
        <div class="col-12">
          <div class="alert alert-info">
            <h6><i class="mdi mdi-information"></i> Butuh Bantuan Lebih Lanjut?</h6>
            <p class="mb-0">
              Hubungi tim Engineering Department atau IT Support untuk panduan lebih detail.
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Flowchart Detail Modal -->
    <div v-if="showModal && currentFlowchart" class="modal-overlay" @click="closeModal">
      <div class="modal-content-large" @click.stop>
        <div class="modal-header" :style="{ backgroundColor: flowcharts[currentFlowchart].color }">
          <h3 class="text-white mb-0">
            {{ flowcharts[currentFlowchart].title }}
          </h3>
          <div class="header-controls">
            <button class="btn-toggle-view" @click="toggleViewMode">
              <i :class="viewMode === 'flowchart' ? 'mdi mdi-format-list-bulleted' : 'mdi mdi-sitemap'"></i>
              {{ viewMode === 'flowchart' ? 'List View' : 'Flowchart View' }}
            </button>
            <button class="btn-close-modal" @click="closeModal">
              <i class="mdi mdi-close"></i>
            </button>
          </div>
        </div>
        <!-- FLOWCHART VIEW -->
        <div v-if="viewMode === 'flowchart'" class="modal-body-flowchart">
          <div class="flowchart-canvas">
            <svg class="flowchart-svg" width="100%" height="100%" style="position: absolute; top: 0; left: 0; pointer-events: none; z-index: 1;">
              <!-- Arrows will be drawn here by Vue -->
            </svg>
            <div class="flowchart-container">
              <div
                v-for="(step, index) in flowcharts[currentFlowchart].steps"
                :key="index"
                class="flowchart-node"
                :class="[
                  `node-${step.type}`,
                  step.condition ? `has-condition` : '',
                  step.branches ? 'has-branches' : ''
                ]"
                :data-step="index"
              >
                <!-- Start/End Node (Rounded Rectangle) -->
                <div v-if="step.type === 'start' || step.type === 'end'" class="node-start-end">
                  <div class="node-icon">
                    <i :class="step.icon"></i>
                  </div>
                  <div class="node-text">{{ step.text }}</div>
                  <div class="node-number">{{ index + 1 }}</div>
                </div>

                <!-- Process Node (Rectangle) -->
                <div v-else-if="step.type === 'process'" class="node-process">
                  <div class="node-icon">
                    <i :class="step.icon"></i>
                  </div>
                  <div class="node-text">{{ step.text }}</div>
                  <div class="node-number">{{ index + 1 }}</div>
                  <!-- Condition Badge -->
                  <div v-if="step.condition" class="condition-badge">{{ step.condition }}</div>
                </div>

                <!-- Decision Node (Diamond) -->
                <div v-else-if="step.type === 'decision'" class="node-decision">
                  <div class="diamond-shape">
                    <div class="diamond-content">
                      <div class="node-icon">
                        <i :class="step.icon"></i>
                      </div>
                      <div class="node-text">{{ step.text }}</div>
                      <div class="node-number">{{ index + 1 }}</div>
                    </div>
                  </div>
                  <!-- Branch Labels -->
                  <div v-if="step.branches" class="branch-labels">
                    <div class="branch-label branch-left">{{ step.branches[0] }}</div>
                    <div class="branch-label branch-right">{{ step.branches[1] }}</div>
                  </div>
                </div>

                <!-- Error Node (Rectangle with X) -->
                <div v-else-if="step.type === 'error'" class="node-error">
                  <div class="node-icon">
                    <i :class="step.icon"></i>
                  </div>
                  <div class="node-text">{{ step.text }}</div>
                  <div class="node-number">{{ index + 1 }}</div>
                </div>

                <!-- Arrow Down (default) -->
                <div v-if="index < flowcharts[currentFlowchart].steps.length - 1" class="arrow-down">
                  <i class="mdi mdi-arrow-down"></i>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- LIST VIEW (Detail Text) -->
        <div v-else class="modal-body-list">
          <div class="list-container">
            <div
              v-for="(step, index) in flowcharts[currentFlowchart].steps"
              :key="index"
              class="list-step-item"
              :class="`step-type-${step.type}`"
            >
              <div class="step-number-badge">{{ index + 1 }}</div>
              <div class="step-icon-large">
                <i :class="step.icon"></i>
              </div>
              <div class="step-details">
                <div class="step-text-large">{{ step.text }}</div>
                <div class="step-meta">
                  <span class="step-type-badge" :class="`badge-${step.type}`">
                    {{ step.type === 'start' ? 'START' : step.type === 'end' ? 'END' : step.type === 'decision' ? 'DECISION' : step.type === 'error' ? 'ERROR' : 'PROCESS' }}
                  </span>
                  <span v-if="step.condition" class="step-condition-badge">{{ step.condition }}</span>
                  <span v-if="step.branches" class="step-branches-badge">
                    <i class="mdi mdi-source-branch"></i> {{ step.branches.join(' / ') }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="closeModal">
            <i class="mdi mdi-close"></i> Tutup
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const currentFlowchart = ref(null);
const showModal = ref(false);
const viewMode = ref('flowchart'); // 'flowchart' atau 'list'

const openFlowchart = (type) => {
  currentFlowchart.value = type;
  showModal.value = true;
  viewMode.value = 'flowchart'; // Default ke flowchart
};

const closeModal = () => {
  showModal.value = false;
  currentFlowchart.value = null;
  viewMode.value = 'flowchart';
};

const toggleViewMode = () => {
  viewMode.value = viewMode.value === 'flowchart' ? 'list' : 'flowchart';
};

// Flowchart data untuk setiap bagian
const flowcharts = {
  login: {
    title: '🔐 Login & Autentikasi',
    color: '#17a2b8',
    steps: [
      { icon: 'mdi-application', text: 'User buka aplikasi Engineering Management', type: 'start' },
      { icon: 'mdi-form-textbox-password', text: 'Masukkan Username & Password', type: 'process' },
      { icon: 'mdi-send', text: 'Frontend POST /login ke Backend', type: 'process' },
      { icon: 'mdi-server-network', text: 'Backend forward ke Auth Server (authserver-backend.iotech.my.id)', type: 'process' },
      { icon: 'mdi-help-circle', text: 'Autentikasi Valid?', type: 'decision', branches: ['Ya', 'Tidak'] },
      { icon: 'mdi-key-variant', text: 'Auth Server return Token', type: 'process', condition: 'Ya' },
      { icon: 'mdi-content-save', text: 'Token disimpan di LocalStorage', type: 'process', condition: 'Ya' },
      { icon: 'mdi-view-dashboard', text: 'Redirect ke Dashboard', type: 'end', condition: 'Ya' },
      { icon: 'mdi-alert-circle', text: 'Tampilkan Error Message', type: 'error', condition: 'Tidak' },
      { icon: 'mdi-reload', text: 'Kembali ke Login Page', type: 'process', condition: 'Tidak' }
    ]
  },
  dashboard: {
    title: '📊 Dashboard',
    color: '#9C27B0',
    steps: [
      { icon: 'mdi-login', text: 'User berhasil login', type: 'start' },
      { icon: 'mdi-view-dashboard', text: 'Akses Dashboard', type: 'process' },
      { icon: 'mdi-api', text: 'GET /api/dashboard/metrics (Data Agregat)', type: 'process' },
      { icon: 'mdi-chart-line', text: 'GET /api/dashboard/ecn-data (Chart ECN)', type: 'process' },
      { icon: 'mdi-chart-bar', text: 'GET /api/dashboard/ccn-data (Chart CCN)', type: 'process' },
      { icon: 'mdi-arrange-bring-forward', text: 'Render Charts & Cards Metrics', type: 'process' },
      { icon: 'mdi-eye', text: 'User lihat: Total ECN/CCN, Manpower, BOM, Charts', type: 'process' },
      { icon: 'mdi-menu', text: 'User pilih menu fitur yang ingin diakses', type: 'end' }
    ]
  },
  ecn: {
    title: '📝 ECN/CCN Management - Complete Flow',
    color: '#1976D2',
    steps: [
      // === PEMBUATAN ECN/CCN ===
      { icon: 'mdi-account-circle', text: '👤 User login dengan role (PIC/Engineer)', type: 'start' },
      { icon: 'mdi-mouse-variant', text: '🖱️ User klik menu "ECN/CCN" di sidebar', type: 'process' },
      { icon: 'mdi-application', text: '📄 Sistem load halaman InputDataPage / EcnPage', type: 'process' },
      { icon: 'mdi-api', text: 'API: GET /engineeringDetailProblems (Load existing ECN list)', type: 'process' },
      { icon: 'mdi-table', text: '📊 Display table list ECN/CCN yang sudah ada', type: 'process' },
      { icon: 'mdi-help-circle', text: '❓ User pilih aksi?', type: 'decision', branches: ['Buat Baru', 'View/Edit Existing'] },

      // Path 1: Buat Baru
      { icon: 'mdi-plus-circle', text: '✨ Klik tombol "Create New ECN/CCN"', type: 'process', condition: 'Buat Baru' },
      { icon: 'mdi-form-textbox', text: '📝 Form muncul dengan fields kosong', type: 'process', condition: 'Buat Baru' },
      { icon: 'mdi-pencil', text: '✍️ User isi Project Name (required)', type: 'process', condition: 'Buat Baru' },
      { icon: 'mdi-text-box-multiple', text: '✍️ User isi Detail Problem (text area)', type: 'process', condition: 'Buat Baru' },
      { icon: 'mdi-note-text', text: '✍️ User isi Engineering Notes (optional)', type: 'process', condition: 'Buat Baru' },
      { icon: 'mdi-format-list-checkbox', text: '☑️ User pilih Type: ECN atau CCN (radio button)', type: 'process', condition: 'Buat Baru' },
      { icon: 'mdi-link-variant', text: '🔗 User pilih/link ke PO ID (optional - dropdown dari CRM)', type: 'process', condition: 'Buat Baru' },
      { icon: 'mdi-playlist-plus', text: '➕ User klik "Add Item" untuk tambah part', type: 'process', condition: 'Buat Baru' },
      { icon: 'mdi-package-variant', text: '📦 Pilih Part Number dari list (API: GET dari PPIC /ext-items)', type: 'process', condition: 'Buat Baru' },
      { icon: 'mdi-counter', text: '🔢 Input Quantity untuk part tersebut', type: 'process', condition: 'Buat Baru' },
      { icon: 'mdi-cash', text: '💰 Input Snapshot Price (harga saat ini)', type: 'process', condition: 'Buat Baru' },
      { icon: 'mdi-arrow-up-down', text: '↕️ Pilih Type: Increase atau Decrease', type: 'process', condition: 'Buat Baru' },
      { icon: 'mdi-help-circle', text: '❓ Tambah item lagi?', type: 'decision', branches: ['Ya - Loop', 'Tidak - Lanjut'], condition: 'Buat Baru' },
      { icon: 'mdi-reload', text: '🔄 Ulangi add item (loop back ke step "Add Item")', type: 'process', condition: 'Ya - Loop' },
      { icon: 'mdi-check-all', text: '✅ Validasi: Semua field required terisi?', type: 'decision', branches: ['Valid', 'Invalid'], condition: 'Tidak - Lanjut' },
      { icon: 'mdi-alert-circle', text: '⚠️ Show validation error messages', type: 'error', condition: 'Invalid' },
      { icon: 'mdi-reload', text: '🔄 Kembali ke form untuk perbaikan', type: 'process', condition: 'Invalid' },
      { icon: 'mdi-send', text: '📤 Frontend: POST /engineeringDetailProblems (JSON data)', type: 'process', condition: 'Valid' },
      { icon: 'mdi-loading', text: '⏳ Loading indicator muncul', type: 'process', condition: 'Valid' },

      // Backend Process
      { icon: 'mdi-server', text: '🖥️ Backend: Terima request di endpoint', type: 'process', condition: 'Valid' },
      { icon: 'mdi-shield-check', text: '🔐 Backend: Validasi token auth dari header', type: 'process', condition: 'Valid' },
      { icon: 'mdi-database-plus', text: '💾 Backend: INSERT ke table EngineeringDetailProblems', type: 'process', condition: 'Valid' },
      { icon: 'mdi-database-settings', text: '💾 Backend: Set Status = "Pending Approval"', type: 'process', condition: 'Valid' },
      { icon: 'mdi-database-settings', text: '💾 Backend: Set CreatedAt = DateTime.Now', type: 'process', condition: 'Valid' },
      { icon: 'mdi-database-settings', text: '💾 Backend: Set ExtUserId = dari token (user yang create)', type: 'process', condition: 'Valid' },
      { icon: 'mdi-database-plus', text: '💾 Backend: INSERT items ke table EngineeringDetailProblemItems', type: 'process', condition: 'Valid' },
      { icon: 'mdi-database-check', text: '💾 Backend: SaveChanges() ke database', type: 'process', condition: 'Valid' },
      { icon: 'mdi-arrow-left', text: '📥 Backend: Return ECN object dengan ID baru ke frontend', type: 'process', condition: 'Valid' },

      // Upload Attachment
      { icon: 'mdi-help-circle', text: '❓ User ingin upload attachment photo?', type: 'decision', branches: ['Ya', 'Tidak'] },
      { icon: 'mdi-file-image', text: '🖼️ User klik button "Upload Photo"', type: 'process', condition: 'Ya' },
      { icon: 'mdi-folder-open', text: '📁 File picker dialog muncul', type: 'process', condition: 'Ya' },
      { icon: 'mdi-file-check', text: '✅ User pilih file image (JPG/PNG)', type: 'process', condition: 'Ya' },
      { icon: 'mdi-send', text: '📤 Frontend: POST /engineeringDetailProblems/{id}/photo (FormData)', type: 'process', condition: 'Ya' },
      { icon: 'mdi-server', text: '🖥️ Backend: Terima file upload', type: 'process', condition: 'Ya' },
      { icon: 'mdi-file-code', text: '🔧 Backend: Generate unique filename (GUID + extension)', type: 'process', condition: 'Ya' },
      { icon: 'mdi-folder-upload', text: '💾 Backend: Save file ke folder files/ di server', type: 'process', condition: 'Ya' },
      { icon: 'mdi-database-edit', text: '💾 Backend: UPDATE record ECN, set ApprovalFileName = filename', type: 'process', condition: 'Ya' },
      { icon: 'mdi-check', text: '✅ Upload success, return filename ke frontend', type: 'process', condition: 'Ya' },

      // Final
      { icon: 'mdi-bell-ring', text: '🔔 (Optional) Send notification ke Manager untuk approval', type: 'process' },
      { icon: 'mdi-check-circle', text: '✅ ECN/CCN Created Successfully! Status: Pending Approval', type: 'process' },
      { icon: 'mdi-refresh', text: '🔄 Reload table list untuk show ECN baru', type: 'process' },
      { icon: 'mdi-information', text: 'ℹ️ Show success message: "ECN/CCN berhasil dibuat, menunggu approval Manager"', type: 'end' },

      // === APPROVAL PROCESS (Separate Flow) ===
      { icon: 'mdi-shield-account', text: '👨‍💼 Manager login dengan role Manager', type: 'start' },
      { icon: 'mdi-mouse-variant', text: '🖱️ Manager klik menu "ECN Approval"', type: 'process' },
      { icon: 'mdi-api', text: 'API: GET /engineeringDetailProblems (filter Status = Pending)', type: 'process' },
      { icon: 'mdi-format-list-bulleted', text: '📋 Display list ECN/CCN yang Pending Approval', type: 'process' },
      { icon: 'mdi-mouse-variant', text: '🖱️ Manager klik salah satu ECN untuk review', type: 'process' },
      { icon: 'mdi-api', text: 'API: GET /engineeringDetailProblems/{id}/all (detailed data)', type: 'process' },
      { icon: 'mdi-database-search', text: '🔍 Backend: Fetch ECN data + Items + Approvals', type: 'process' },
      { icon: 'mdi-api', text: 'API: Fetch user data dari Auth Server (untuk Created By name)', type: 'process' },
      { icon: 'mdi-api', text: 'API: Fetch PO data dari CRM (jika ada PO linked)', type: 'process' },
      { icon: 'mdi-api', text: 'API: Fetch Items detail dari PPIC (part info)', type: 'process' },
      { icon: 'mdi-eye', text: '👁️ Manager review: Project, Problem, Items, Price, Photo', type: 'process' },
      { icon: 'mdi-help-circle', text: '❓ Manager decision?', type: 'decision', branches: ['Approve', 'Reject'] },

      // Approve Path
      { icon: 'mdi-check-circle-outline', text: '✅ Manager klik button "Approve"', type: 'process', condition: 'Approve' },
      { icon: 'mdi-text-box', text: '📝 Manager input Approval Remark (optional)', type: 'process', condition: 'Approve' },
      { icon: 'mdi-calendar', text: '📅 System set Approval Date = Today', type: 'process', condition: 'Approve' },
      { icon: 'mdi-send', text: '📤 Frontend: POST/PUT approval data', type: 'process', condition: 'Approve' },
      { icon: 'mdi-database-edit', text: '💾 Backend: UPDATE EngineeringDetailProblem.Status = "Approved"', type: 'process', condition: 'Approve' },
      { icon: 'mdi-database-edit', text: '💾 Backend: SET ApprovalDate, ApprovalRemark', type: 'process', condition: 'Approve' },
      { icon: 'mdi-database-plus', text: '💾 Backend: INSERT ke EngineeringDetailProblemApprovals (history)', type: 'process', condition: 'Approve' },
      { icon: 'mdi-check-circle', text: '✅ Approval Success!', type: 'process', condition: 'Approve' },

      // Reject Path
      { icon: 'mdi-close-circle-outline', text: '❌ Manager klik button "Reject"', type: 'process', condition: 'Reject' },
      { icon: 'mdi-text-box', text: '📝 Manager WAJIB input Rejection Remark (alasan)', type: 'process', condition: 'Reject' },
      { icon: 'mdi-check-all', text: '✅ Validasi: Remark tidak boleh kosong', type: 'decision', branches: ['Ada', 'Kosong'], condition: 'Reject' },
      { icon: 'mdi-alert', text: '⚠️ Error: "Alasan reject harus diisi!"', type: 'error', condition: 'Kosong' },
      { icon: 'mdi-send', text: '📤 Frontend: POST/PUT rejection data', type: 'process', condition: 'Ada' },
      { icon: 'mdi-database-edit', text: '💾 Backend: UPDATE Status = "Rejected"', type: 'process', condition: 'Ada' },
      { icon: 'mdi-database-edit', text: '💾 Backend: SET ApprovalRemark = alasan reject', type: 'process', condition: 'Ada' },
      { icon: 'mdi-database-plus', text: '💾 Backend: INSERT rejection history ke Approvals table', type: 'process', condition: 'Ada' },
      { icon: 'mdi-close-circle', text: '❌ ECN/CCN Rejected!', type: 'process', condition: 'Ada' },

      // Notification
      { icon: 'mdi-bell-ring', text: '🔔 System send notification ke Creator (PIC yang buat ECN)', type: 'process' },
      { icon: 'mdi-email', text: '📧 (Optional) Send email notification', type: 'process' },
      { icon: 'mdi-check-decagram', text: '🎉 Process Complete! ECN Status Updated', type: 'end' }
    ]
  },
  activity: {
    title: '⚙️ Activity & Task Creation',
    color: '#FF9800',
    steps: [
      { icon: 'mdi-clipboard-list', text: 'User akses Activity Page (Activity.vue)', type: 'start' },
      { icon: 'mdi-form-select', text: 'Isi Activity Form: Type (PrePO/PostPO/Others), Customer, Description', type: 'process' },
      { icon: 'mdi-link-variant', text: 'Link ke PO/Inquiry/ECN jika applicable', type: 'process' },
      { icon: 'mdi-playlist-plus', text: 'Tambah Multiple Tasks', type: 'process' },
      { icon: 'mdi-text-box', text: 'Untuk setiap Task: Description, Hours, Due Date, Remark', type: 'process' },
      { icon: 'mdi-account-multiple', text: 'Assign PIC (Multiple In-Charges per Task)', type: 'process' },
      { icon: 'mdi-calculator', text: 'Frontend calculate FromCache & ToCache dari Task Dates', type: 'process' },
      { icon: 'mdi-send', text: 'POST /api/dashboard/activities', type: 'process' },
      { icon: 'mdi-database', text: 'Backend save: EngineeringActivity + Tasks + InCharges', type: 'process' },
      { icon: 'mdi-bell-ring', text: 'Create Notification untuk PIC pertama (CurrentRole: PIC)', type: 'process' },
      { icon: 'mdi-check-circle', text: 'Activity & Tasks Created Successfully', type: 'end' }
    ]
  },
  'task-execution': {
    title: '✅ Task Execution Flow',
    color: '#4CAF50',
    steps: [
      { icon: 'mdi-bell', text: 'Notification Created (CurrentRole: PIC)', type: 'start' },
      { icon: 'mdi-account', text: 'PIC menerima notifikasi', type: 'process' },
      { icon: 'mdi-api', text: 'GET /api/notifications/active?role=PIC', type: 'process' },
      { icon: 'mdi-eye', text: 'PIC lihat task details', type: 'process' },
      { icon: 'mdi-briefcase', text: 'PIC kerjakan task', type: 'process' },
      { icon: 'mdi-check-bold', text: 'PIC mark Done - POST /api/notifications/done', type: 'process' },
      { icon: 'mdi-update', text: 'Update CompletedDatePic & CompletedByPicId', type: 'process' },
      { icon: 'mdi-bell-ring', text: 'Create Notification untuk SPV (CurrentRole: SPV)', type: 'process' },
      { icon: 'mdi-account-tie', text: 'SPV menerima notifikasi', type: 'process' },
      { icon: 'mdi-api', text: 'GET /api/notifications/active?role=SPV', type: 'process' },
      { icon: 'mdi-eye-check', text: 'SPV review task', type: 'process' },
      { icon: 'mdi-check-bold', text: 'SPV mark Done - POST /api/notifications/done', type: 'process' },
      { icon: 'mdi-update', text: 'Update CompletedDateSpv & CompletedBySpvId', type: 'process' },
      { icon: 'mdi-bell-ring', text: 'Create Notification untuk Manager (CurrentRole: Manager)', type: 'process' },
      { icon: 'mdi-account-star', text: 'Manager menerima notifikasi', type: 'process' },
      { icon: 'mdi-api', text: 'GET /api/notifications/active?role=Manager', type: 'process' },
      { icon: 'mdi-eye-check', text: 'Manager review task', type: 'process' },
      { icon: 'mdi-check-bold', text: 'Manager mark Done - POST /api/notifications/done', type: 'process' },
      { icon: 'mdi-update', text: 'Update CompletedDateManager & CompletedByManagerId', type: 'process' },
      { icon: 'mdi-check-all', text: 'Update Notification Status: Completed', type: 'process' },
      { icon: 'mdi-trophy', text: 'Task Fully Completed! 🎉', type: 'end' }
    ]
  },
  bom: {
    title: '✅ BOM Approval Process',
    color: '#009688',
    steps: [
      { icon: 'mdi-alert-decagram', text: 'BOM Change detected dari PPIC Backend', type: 'start' },
      { icon: 'mdi-database-plus', text: 'Backend create BomApproval Record', type: 'process' },
      { icon: 'mdi-account-multiple-plus', text: 'Assign Multiple PICs (Create BomApprovalPic Records)', type: 'process' },
      { icon: 'mdi-view-list', text: 'PICs akses BOMApproval Page', type: 'process' },
      { icon: 'mdi-eye', text: 'Each PIC review BOM Details', type: 'process' },
      { icon: 'mdi-pencil', text: 'PIC submit Approval dengan Remark & Date', type: 'process' },
      { icon: 'mdi-update', text: 'Update BomApprovalPic Record', type: 'process' },
      { icon: 'mdi-help-circle', text: 'Semua PICs sudah Approve?', type: 'decision', branches: ['Ya', 'Tidak'] },
      { icon: 'mdi-clock-outline', text: 'Wait for Other PICs', type: 'process', condition: 'Tidak' },
      { icon: 'mdi-camera', text: 'Generate BOM Snapshot', type: 'process', condition: 'Ya' },
      { icon: 'mdi-send', text: 'POST Snapshot ke PPIC Backend', type: 'process', condition: 'Ya' },
      { icon: 'mdi-check-decagram', text: 'Update BomApproval Status: Approved', type: 'process', condition: 'Ya' },
      { icon: 'mdi-bell-ring', text: 'Send Notifications ke All Stakeholders', type: 'process', condition: 'Ya' },
      { icon: 'mdi-check-circle', text: 'BOM Approval Complete', type: 'end', condition: 'Ya' }
    ]
  },
  support: {
    title: '📁 Support Document Management',
    color: '#E91E63',
    steps: [
      { icon: 'mdi-folder-open', text: 'User akses SupportDoc Page (SupportDoc.vue)', type: 'start' },
      { icon: 'mdi-api', text: 'GET /supporttables - View Support Tables List', type: 'process' },
      { icon: 'mdi-help-circle', text: 'Pilih Aksi?', type: 'decision', branches: ['Upload', 'Link', 'Download', 'View'] },
      { icon: 'mdi-file-upload', text: 'Select File & Enter Name', type: 'process', condition: 'Upload' },
      { icon: 'mdi-send', text: 'POST /supporttables dengan FormData', type: 'process', condition: 'Upload' },
      { icon: 'mdi-content-save', text: 'Save File ke uploads/ folder dengan unique filename', type: 'process', condition: 'Upload' },
      { icon: 'mdi-database', text: 'Save Record ke SupportTables', type: 'process', condition: 'Upload' },
      { icon: 'mdi-link-variant', text: 'Enter JobId & Select SupportTableIds', type: 'process', condition: 'Link' },
      { icon: 'mdi-send', text: 'POST /api/support-engineering-documents', type: 'process', condition: 'Link' },
      { icon: 'mdi-database', text: 'Save to SupportEngineeringDocuments', type: 'process', condition: 'Link' },
      { icon: 'mdi-download', text: 'GET /supporttables/{id}/download', type: 'process', condition: 'Download' },
      { icon: 'mdi-file-download', text: 'File Downloaded', type: 'end', condition: 'Download' },
      { icon: 'mdi-eye', text: 'GET /api/support-engineering-documents/job/{jobId}', type: 'process', condition: 'View' },
      { icon: 'mdi-view-list', text: 'Display Linked Documents', type: 'end', condition: 'View' }
    ]
  },
  ai: {
    title: '🤖 AI Document Analyzer',
    color: '#673AB7',
    steps: [
      { icon: 'mdi-robot', text: 'User akses AI Analyzer Page (AIDocumentAnalyzerPage.vue)', type: 'start' },
      { icon: 'mdi-help-circle', text: 'Select Input Type?', type: 'decision', branches: ['Document', 'Inquiry'] },
      { icon: 'mdi-file-upload', text: 'Upload Document from Computer', type: 'process', condition: 'Document' },
      { icon: 'mdi-send', text: 'Upload to Backend', type: 'process', condition: 'Document' },
      { icon: 'mdi-format-list-numbered', text: 'Choose Inquiry ID from List', type: 'process', condition: 'Inquiry' },
      { icon: 'mdi-api', text: 'Backend fetch Inquiry Data dari CRM API', type: 'process', condition: 'Inquiry' },
      { icon: 'mdi-brain', text: 'Send to AI for Analysis', type: 'process' },
      { icon: 'mdi-cog', text: 'AI Process Document/Inquiry Content', type: 'process' },
      { icon: 'mdi-text-box-multiple', text: 'Generate Analysis Results', type: 'process' },
      { icon: 'mdi-database', text: 'Save to InquiryAIGenerations Table', type: 'process' },
      { icon: 'mdi-file-document', text: 'Display Results dalam Markdown Format', type: 'process' },
      { icon: 'mdi-help-circle', text: 'Use Results?', type: 'decision', branches: ['Eng Notes', 'Tasks', 'Review'] },
      { icon: 'mdi-content-copy', text: 'Copy to Engineering Notes', type: 'end', condition: 'Eng Notes' },
      { icon: 'mdi-playlist-plus', text: 'Generate Task Descriptions', type: 'end', condition: 'Tasks' },
      { icon: 'mdi-pencil', text: 'Manual Review & Edit', type: 'end', condition: 'Review' }
    ]
  },
  reports: {
    title: '📈 Reports & Export',
    color: '#FF5722',
    steps: [
      { icon: 'mdi-chart-bar', text: 'User akses Report Page', type: 'start' },
      { icon: 'mdi-help-circle', text: 'Pilih Jenis Report?', type: 'decision', branches: ['ECN/CCN', 'Activity', 'Manpower'] },
      { icon: 'mdi-filter', text: 'Set Filter: Date Range, User, Inquiry, dll', type: 'process' },
      { icon: 'mdi-api', text: 'GET /api/dashboard/activities dengan parameters', type: 'process' },
      { icon: 'mdi-database-search', text: 'Backend fetch Data berdasarkan Filter', type: 'process' },
      { icon: 'mdi-link-variant', text: 'Fetch External Data: Users (Auth), POs (CRM), Items (PPIC)', type: 'process' },
      { icon: 'mdi-help-circle', text: 'Export Format?', type: 'decision', branches: ['View', 'Excel'] },
      { icon: 'mdi-eye', text: 'Display in Browser', type: 'end', condition: 'View' },
      { icon: 'mdi-file-excel', text: 'Backend generate Excel dengan ClosedXML Library', type: 'process', condition: 'Excel' },
      { icon: 'mdi-table', text: 'Populate Excel Sheet dengan Data', type: 'process', condition: 'Excel' },
      { icon: 'mdi-download', text: 'Download Excel File', type: 'end', condition: 'Excel' }
    ]
  },
  notifications: {
    title: '🔔 Notification System',
    color: '#03A9F4',
    steps: [
      { icon: 'mdi-clipboard-plus', text: 'Task Created (POST /api/tasks)', type: 'start' },
      { icon: 'mdi-bell-plus', text: 'Notification otomatis dibuat untuk PIC aktif (CurrentRole: PIC)', type: 'process' },
      { icon: 'mdi-account', text: 'PIC login ke aplikasi', type: 'process' },
      { icon: 'mdi-bell', text: 'PIC klik icon Notification di Navbar', type: 'process' },
      { icon: 'mdi-api', text: 'GET /api/notifications/active?role=PIC', type: 'process' },
      { icon: 'mdi-view-list', text: 'Display list tasks yang assigned ke PIC', type: 'process' },
      { icon: 'mdi-cursor-pointer', text: 'PIC klik notification untuk detail', type: 'process' },
      { icon: 'mdi-check-bold', text: 'PIC mark as Done (POST /api/notifications/done)', type: 'process' },
      { icon: 'mdi-bell-ring', text: 'Notification auto-created untuk role berikutnya (SPV)', type: 'process' },
      { icon: 'mdi-help-circle', text: 'Need to Undo?', type: 'decision', branches: ['Ya', 'Tidak'] },
      { icon: 'mdi-undo', text: 'PUT /api/notifications/undone - Rollback status', type: 'process', condition: 'Ya' },
      { icon: 'mdi-reload', text: 'Status kembali ke tahap sebelumnya', type: 'process', condition: 'Ya' },
      { icon: 'mdi-bell-check', text: 'Notification updated (IsRead = true saat dibuka)', type: 'process', condition: 'Tidak' },
      { icon: 'mdi-check-circle', text: 'Workflow continues sampai Manager approve', type: 'end' }
    ]
  }
};
</script>

<style scoped>
.user-guide-page {
  background-color: #f5f5f5;
  min-height: 100vh;
}

.card {
  border-radius: 10px;
  transition: transform 0.2s, box-shadow 0.2s;
}

.guide-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 16px rgba(0,0,0,0.2) !important;
}

.card-header {
  border-top-left-radius: 10px;
  border-top-right-radius: 10px;
  font-weight: 600;
}

.bg-purple {
  background-color: #9C27B0;
}

.bg-teal {
  background-color: #009688;
}

.bg-pink {
  background-color: #E91E63;
}

.bg-deep-purple {
  background-color: #673AB7;
}

.bg-orange {
  background-color: #FF9800;
}

.bg-light-blue {
  background-color: #03A9F4;
}

.flow-diagram {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.flow-step {
  display: flex;
  align-items: center;
  gap: 20px;
  padding: 20px;
  background: white;
  border: 2px solid #ddd;
  border-radius: 10px;
  margin: 10px 0;
  transition: all 0.3s;
}

.flow-step:hover {
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
  transform: translateX(5px);
}

.step-icon {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  color: white;
  flex-shrink: 0;
}

.step-content {
  flex: 1;
}

.step-content h6 {
  margin: 0 0 5px 0;
  font-weight: 600;
  color: #333;
}

.step-content p {
  margin: 0;
  color: #666;
  font-size: 0.9rem;
}

.flow-arrow {
  text-align: center;
  font-size: 32px;
  color: #666;
  margin: 10px 0;
}

.flow-branches {
  padding: 30px 20px;
  background: #f8f9fa;
  border-radius: 10px;
  margin: 10px 0;
}

.branch {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 15px;
}

.branch-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 15px;
  background: white;
  border-radius: 8px;
  border: 1px solid #ddd;
  transition: all 0.3s;
  cursor: pointer;
}

.branch-item:hover {
  transform: translateY(-3px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.1);
}

.branch-item i {
  font-size: 32px;
}

.branch-item span {
  font-size: 0.85rem;
  font-weight: 500;
  text-align: center;
}

.text-purple {
  color: #9C27B0;
}

.text-orange {
  color: #FF9800;
}

.clickable {
  cursor: pointer;
  transition: all 0.3s;
  position: relative;
}

.clickable:hover {
  transform: translateX(8px);
  box-shadow: 0 6px 20px rgba(0,0,0,0.2) !important;
}

.click-hint {
  position: absolute;
  right: 15px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 0.75rem;
  color: #666;
  opacity: 0;
  transition: opacity 0.3s;
}

.clickable:hover .click-hint {
  opacity: 1;
}

.click-hint-small {
  display: block;
  font-size: 0.7rem;
  color: #888;
  margin-top: 4px;
  opacity: 0.7;
}

.branch-item {
  cursor: pointer;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  padding: 20px;
  animation: fadeIn 0.3s;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.modal-content-large {
  background: white;
  border-radius: 15px;
  max-width: 900px;
  width: 100%;
  max-height: 90vh;
  display: flex;
  flex-direction: column;
  box-shadow: 0 10px 40px rgba(0,0,0,0.3);
  animation: slideUp 0.3s;
}

@keyframes slideUp {
  from {
    transform: translateY(50px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.modal-header {
  padding: 20px 30px;
  border-top-left-radius: 15px;
  border-top-right-radius: 15px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-controls {
  display: flex;
  gap: 10px;
  align-items: center;
}

.btn-toggle-view {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 2px solid rgba(255, 255, 255, 0.5);
  padding: 8px 16px;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s;
  font-size: 0.9rem;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 6px;
}

.btn-toggle-view:hover {
  background: rgba(255, 255, 255, 0.3);
  border-color: white;
  transform: scale(1.05);
}

.btn-close-modal {
  background: rgba(255,255,255,0.2);
  border: none;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  color: white;
  font-size: 24px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s;
}

.btn-close-modal:hover {
  background: rgba(255,255,255,0.3);
  transform: rotate(90deg);
}

.modal-body-flowchart {
  padding: 30px;
  overflow-y: auto;
  overflow-x: auto;
  flex: 1;
  background: #fafafa;
}

.flowchart-canvas {
  position: relative;
  min-height: 600px;
  padding: 20px;
}

.flowchart-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 20px;
  position: relative;
  z-index: 2;
}

.flowchart-node {
  display: flex;
  flex-direction: column;
  align-items: center;
  position: relative;
}

/* ========== START/END NODE (Rounded Rectangle) ========== */
.node-start-end {
  background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%);
  color: white;
  padding: 20px 40px;
  border-radius: 50px;
  min-width: 300px;
  max-width: 600px;
  box-shadow: 0 4px 15px rgba(76, 175, 80, 0.3);
  text-align: center;
  position: relative;
  border: 3px solid #2E7D32;
  transition: all 0.3s;
}

.node-start-end:hover {
  transform: scale(1.05);
  box-shadow: 0 6px 20px rgba(76, 175, 80, 0.4);
}

/* ========== PROCESS NODE (Rectangle) ========== */
.node-process {
  background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%);
  color: white;
  padding: 18px 30px;
  border-radius: 8px;
  min-width: 280px;
  max-width: 600px;
  box-shadow: 0 3px 12px rgba(33, 150, 243, 0.3);
  text-align: center;
  position: relative;
  border: 2px solid #1565C0;
  transition: all 0.3s;
}

.node-process:hover {
  transform: translateY(-3px);
  box-shadow: 0 5px 18px rgba(33, 150, 243, 0.4);
}

/* ========== DECISION NODE (Diamond) ========== */
.node-decision {
  display: flex;
  flex-direction: column;
  align-items: center;
  position: relative;
  margin: 20px 0;
}

.diamond-shape {
  width: 180px;
  height: 180px;
  background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%);
  transform: rotate(45deg);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 15px rgba(255, 152, 0, 0.4);
  border: 3px solid #E65100;
  position: relative;
  transition: all 0.3s;
}

.diamond-shape:hover {
  transform: rotate(45deg) scale(1.1);
  box-shadow: 0 6px 20px rgba(255, 152, 0, 0.5);
}

.diamond-content {
  transform: rotate(-45deg);
  color: white;
  text-align: center;
  padding: 15px;
  width: 140px;
}

.branch-labels {
  display: flex;
  justify-content: space-between;
  width: 400px;
  margin-top: -80px;
  position: relative;
  z-index: 10;
}

.branch-label {
  background: #FF9800;
  color: white;
  padding: 8px 20px;
  border-radius: 20px;
  font-weight: bold;
  font-size: 0.9rem;
  box-shadow: 0 2px 8px rgba(255, 152, 0, 0.3);
  border: 2px solid #F57C00;
}

.branch-left {
  margin-right: auto;
}

.branch-right {
  margin-left: auto;
}

/* ========== ERROR NODE (Red Rectangle) ========== */
.node-error {
  background: linear-gradient(135deg, #F44336 0%, #D32F2F 100%);
  color: white;
  padding: 18px 30px;
  border-radius: 8px;
  min-width: 280px;
  max-width: 600px;
  box-shadow: 0 3px 12px rgba(244, 67, 54, 0.3);
  text-align: center;
  position: relative;
  border: 2px solid #C62828;
  transition: all 0.3s;
}

.node-error:hover {
  transform: translateY(-3px);
  box-shadow: 0 5px 18px rgba(244, 67, 54, 0.4);
}

.node-error::before {
  content: '⚠️';
  position: absolute;
  left: 15px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 24px;
}

/* ========== COMMON NODE ELEMENTS ========== */
.node-icon {
  font-size: 24px;
  margin-bottom: 8px;
  opacity: 0.9;
}

.node-text {
  font-size: 0.95rem;
  line-height: 1.4;
  font-weight: 500;
}

.node-number {
  position: absolute;
  top: -12px;
  right: -12px;
  background: white;
  color: #333;
  width: 28px;
  height: 28px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 0.85rem;
  border: 2px solid #666;
  box-shadow: 0 2px 6px rgba(0,0,0,0.2);
  z-index: 5;
}

.condition-badge {
  position: absolute;
  top: -10px;
  left: 15px;
  background: #FFC107;
  color: #333;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: bold;
  border: 2px solid #FFA000;
  box-shadow: 0 2px 6px rgba(255, 193, 7, 0.3);
  z-index: 5;
}

/* ========== ARROWS ========== */
.arrow-down {
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 5px 0;
  color: #666;
  font-size: 32px;
  animation: arrowPulse 2s infinite;
}

@keyframes arrowPulse {
  0%, 100% {
    transform: translateY(0);
    opacity: 0.6;
  }
  50% {
    transform: translateY(5px);
    opacity: 1;
  }
}

/* ========== CONDITIONAL STYLING ========== */
.has-condition {
  position: relative;
}

/* Different colors for different condition types */
.has-condition .node-process[data-condition*="Ya"],
.has-condition .node-process[data-condition*="Valid"],
.has-condition .node-process[data-condition*="Approve"] {
  border-left: 5px solid #4CAF50;
}

.has-condition .node-process[data-condition*="Tidak"],
.has-condition .node-process[data-condition*="Invalid"],
.has-condition .node-process[data-condition*="Reject"] {
  border-left: 5px solid #F44336;
}

/* ========== RESPONSIVE ========== */
@media (max-width: 768px) {
  .node-start-end,
  .node-process,
  .node-error {
    min-width: 200px;
    max-width: 90%;
    padding: 15px 20px;
  }

  .diamond-shape {
    width: 140px;
    height: 140px;
  }

  .diamond-content {
    width: 100px;
    padding: 10px;
  }

  .branch-labels {
    width: 300px;
  }

  .node-text {
    font-size: 0.85rem;
  }
}

/* ========== LIST VIEW STYLES ========== */
.modal-body-list {
  padding: 30px;
  overflow-y: auto;
  flex: 1;
  background: #fafafa;
}

.list-container {
  display: flex;
  flex-direction: column;
  gap: 15px;
  max-width: 800px;
  margin: 0 auto;
}

.list-step-item {
  background: white;
  border-radius: 12px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 15px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: all 0.3s;
  position: relative;
}

.list-step-item:hover {
  transform: translateX(5px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

/* Different border colors based on step type */
.step-type-start {
  border-left: 5px solid #4CAF50;
}

.step-type-end {
  border-left: 5px solid #4CAF50;
}

.step-type-process {
  border-left: 5px solid #2196F3;
}

.step-type-decision {
  border-left: 5px solid #FF9800;
}

.step-type-error {
  border-left: 5px solid #F44336;
}

.step-number-badge {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 1.1rem;
  flex-shrink: 0;
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3);
}

.step-icon-large {
  font-size: 32px;
  color: #666;
  flex-shrink: 0;
}

.step-details {
  flex: 1;
}

.step-text-large {
  font-size: 1rem;
  line-height: 1.5;
  color: #333;
  margin-bottom: 8px;
  font-weight: 500;
}

.step-meta {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
  align-items: center;
}

.step-type-badge {
  padding: 4px 10px;
  border-radius: 6px;
  font-size: 0.75rem;
  font-weight: bold;
  text-transform: uppercase;
}

.badge-start {
  background: #4CAF50;
  color: white;
}

.badge-end {
  background: #4CAF50;
  color: white;
}

.badge-process {
  background: #2196F3;
  color: white;
}

.badge-decision {
  background: #FF9800;
  color: white;
}

.badge-error {
  background: #F44336;
  color: white;
}

.step-condition-badge {
  background: #FFC107;
  color: #333;
  padding: 4px 10px;
  border-radius: 6px;
  font-size: 0.75rem;
  font-weight: bold;
}

.step-branches-badge {
  background: #E1BEE7;
  color: #6A1B9A;
  padding: 4px 10px;
  border-radius: 6px;
  font-size: 0.75rem;
  font-weight: bold;
  display: flex;
  align-items: center;
  gap: 4px;
}

/* ========== OLD STYLES - CLEANED UP ========== */

.modal-footer {
  padding: 15px 30px;
  border-top: 1px solid #e0e0e0;
  display: flex;
  justify-content: flex-end;
}

.alert {
  border-radius: 8px;
  font-size: 0.9rem;
}

.accordion-button {
  font-weight: 500;
}

.accordion-button:not(.collapsed) {
  background-color: #e3f2fd;
  color: #1976d2;
}

@media (max-width: 768px) {
  .card-body ol, .card-body ul {
    padding-left: 20px;
    font-size: 0.9rem;
  }
}
</style>
