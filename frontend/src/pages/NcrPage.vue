<template>
    <div class="ncr-page-container">
      <iframe
        :src="ncrPageUrl"
        title="Halaman NCR"
        class="ncr-iframe"
      ></iframe>
    </div>
  </template>
  
  <script>
  export default {
    name: 'NcrPage',
    props: {
      // userId akan diteruskan sebagai prop ke komponen ini
      // Ini mirip dengan bagaimana `userId` diambil dari `AppContext` di contoh React Anda.
      userId: {
        type: [String, Number],
        default: 0 // Nilai default jika userId tidak disediakan
      }
    },
    computed: {
      ncrPageUrl() {
        // Base URL dari halaman NCR yang ingin disematkan
        const baseUrl = 'https://qaqc.iotech.my.id/#/ncr';
        // Membangun URL lengkap dengan userId
        // Pastikan userId memiliki nilai, jika tidak, gunakan default 0
        const userIdentifier = this.userId !== null && this.userId !== undefined ? this.userId : 0;
        return `${baseUrl}?meetingAppUserId=${userIdentifier}`;
      }
    }
  }
  </script>
  
  <style scoped>
  .ncr-page-container {
    height: 85vh; /* Mengatur tinggi kontainer, mirip dengan contoh React */
    width: 100%; /* Mengatur lebar kontainer */
    box-sizing: border-box; /* Memastikan padding dan border disertakan dalam lebar/tinggi */
    display: flex; /* Menggunakan flexbox untuk penataan iframe */
    justify-content: center; /* Memusatkan iframe secara horizontal */
    align-items: center; /* Memusatkan iframe secara vertikal */
  }
  
  .ncr-iframe {
    border: 2px solid #ccc; /* Border abu-abu, mirip dengan contoh React */
    border-radius: 8px; /* Sudut membulat */
    height: 100%; /* Iframe mengisi tinggi kontainer */
    width: 100%; /* Iframe mengisi lebar kontainer */
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); /* Menambahkan sedikit bayangan untuk tampilan lebih baik */
  }
  </style>
  