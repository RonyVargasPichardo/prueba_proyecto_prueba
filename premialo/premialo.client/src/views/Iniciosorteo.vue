
<template>
  <div class="ruleta-wall">
    <div
      v-for="(columna, i) in columnas"
      :key="i"
      class="ruleta-columna"
      :style="{ '--offset': columna.offset + 'px', '--i': i }"
    >
      <div class="ruleta-wrap">
        <div
          v-for="(nombre, j) in columna.nombres"
          :key="i + '-' + j"
          class="nombre"
        >
          {{ nombre }}
        </div>

        <!-- duplicado para loop infinito -->
        <div
          v-for="(nombre, j) in columna.nombres"
          :key="'b' + i + '-' + j"
          class="nombre"
        >
          {{ nombre }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";

const usuarios = ref([]);
const columnas = ref([]);
const numColumnas = 6;

// 🔹 Mezclador aleatorio (Fisher–Yates)
function shuffle(array) {
  const arr = [...array];
  for (let i = arr.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    [arr[i], arr[j]] = [arr[j], arr[i]];
  }
  return arr;
}

onMounted(async () => {
  const res = await fetch("/usuarios.json");
  usuarios.value = await res.json();

  const base = usuarios.value.map(u => `${u.Nombre} ${u.Apellido}`);

  // 🔹 Crear columnas con orden y offset independientes
  columnas.value = Array.from({ length: numColumnas }, (_, i) => {
    const nombresMezclados = shuffle(base);
    const nombresExtendidos = Array.from({ length: 10 }, () => nombresMezclados).flat();
    return {
      nombres: nombresExtendidos,
      offset: Math.floor(Math.random() * 600) * (Math.random() > 0.5 ? 1 : -1),
      i,
    };
  });
});
</script>

<style scoped>
.ruleta-wall {
  position: relative;
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  width: 30000vw;
  overflow: hidden;
  background: radial-gradient(circle at center, #001a33 0%, #000814 100%);
  perspective: 200px;
  transform: translateX(490px);
}

/* 🔹 Cada columna inclinada */
.ruleta-columna {
  flex: 0 0 450px;
  display: flex;
  flex-direction: column;
  align-items: center;
  overflow: hidden;
  position: relative;
  transform-origin: center center;
  -webkit-mask-image: linear-gradient(to bottom, transparent 5%, rgba(0,0,0,1) 25%, rgba(0,0,0,1) 75%, transparent 95%);
  mask-image: linear-gradient(to bottom, transparent 5%, rgba(0,0,0,1) 25%, rgba(0,0,0,1) 75%, transparent 95%);
  will-change: transform;
}

/* 🔹 Distribución horizontal */
.ruleta-columna {
  transform: rotate(-20deg)
    translateX(calc((var(--i) - 10) * 6rem))
    translateZ(calc((10 - var(--i)) * -10px));
}

/* 🔹 Animación vertical (más eficiente) */
.ruleta-wrap {
  display: flex;
  flex-direction: column;
  transform: translateY(var(--offset, 0));
  animation-duration: 400s; /* 🟢 más rápido y ligero */
  animation-timing-function: linear;
  animation-iteration-count: infinite;
  will-change: transform;
}

/* 🔹 Alternamos dirección */
.ruleta-columna:nth-child(odd) .ruleta-wrap {
  animation-name: scrollUp;
}
.ruleta-columna:nth-child(even) .ruleta-wrap {
  animation-name: scrollDown;
}

/* 🔹 Estilo de nombres */
.nombre {
  color: #ffffff;
  font-size: 2.5rem;
  font-weight: 600;
  text-align: center;
  padding: 1rem 1rem;
  text-shadow: 0 0 20px rgba(0, 255, 255, 0.4);
  font-family: "Segoe UI", sans-serif;
  white-space: nowrap;
  backface-visibility: hidden; /* 🟢 evita repaints */
  will-change: transform;
}

/* 🔹 Hover opcional */
/* .nombre:hover {
  text-shadow: 0 0 25px rgba(255, 255, 255, 0.9);
  transform: scale(1.05);
} */

/* 🔹 Animaciones GPU-friendly */
@keyframes scrollUp {
  from {
    transform: translate3d(0, 0, 0);
  }
  to {
    transform: translate3d(0, -50%, 0);
  }
}

@keyframes scrollDown {
  from {
    transform: translate3d(0, -50%, 0);
  }
  to {
    transform: translate3d(0, 0, 0);
  }
}
</style>

