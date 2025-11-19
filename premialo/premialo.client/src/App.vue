<template>
  <Toast />
  <!-- 🌌 Fondo con nieve cayendo -->
  <div class="app-container">
    <!-- ❄️ Capa global de nieve -->
    <div class="snow-container">
      <span
        v-for="n in 80"
        :key="n"
        class="snowflake"
        :style="{
          left: Math.random() * 100 + '%',
          animationDuration: (Math.random() * 5 + 5) + 's',
          fontSize: (Math.random() * 12 + 8) + 'px',
          animationDelay: (Math.random() * 5) + 's',
          opacity: (Math.random() * 0.6 + 0.4)
        }"
      >
      <i class="fa-regular fa-snowflake"></i>
      </span>
    </div>

    <!-- Vistas dinámicas del router -->
    <router-view />
  </div>
</template>

<script>

export default {
  name: "App",
};
</script>

<style>
/* 🌌 Fondo base institucional */
.app-container {
  position: relative;
  min-height: 100vh;
  width: 100%;
  overflow: hidden;
  background: linear-gradient(to bottom right, #003870, #001f3f);
  display: flex;
  justify-content: center;
  align-items: center;
}

/* 🌨️ Nieve global */
.snow-container {
  position: fixed;
  inset: 0;
  pointer-events: none;
  overflow: hidden;
  z-index: 9999;
}
.snowflake {
  position: absolute;
  color: #fff;
  top: -10px;
  animation: caer linear infinite;
  filter: drop-shadow(0 0 6px rgba(255,255,255,0.8));
}
@keyframes caer {
  0% { transform: translateY(-10px) rotate(0deg); }
  100% { transform: translateY(100vh) rotate(360deg); }
}
</style>
