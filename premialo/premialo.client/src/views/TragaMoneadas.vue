<template>
  <div class="slot-machine">
    <h2 class="title">🎄 SORTEO NAVIDEÑO DE EMPLEADOS 🎁</h2>

    <button class="btn" @click="iniciarSorteo" :disabled="girando">
      {{ girando ? "Girando..." : "🎰 Iniciar Sorteo" }}
    </button>

    <div class="slots">
      <div v-for="(slot, i) in slots" :key="i" class="slot">
        <h3>{{ slot.title }}</h3>

        <div class="slot-window">
          <div class="highlight-line"></div>

          <div
            class="slot-wrap"
            :style="{ transform: `translateY(${slot.position}px)` }"
          >
            <div
              v-for="(item, j) in slot.items"
              :key="j"
              class="slot-item"
            >
              {{ item }}
            </div>

            <!-- duplicado para loop -->
            <div
              v-for="(item, j) in slot.items"
              :key="'b'+j"
              class="slot-item"
            >
              {{ item }}
            </div>
          </div>
        </div>
      </div>
    </div>

    <transition name="fade">
      <div v-if="ganador" class="winner-card">
        <h2>🏆 ¡GANADOR FINAL!</h2>
        <p><strong>Departamento:</strong> {{ ganador.Departamento }}</p>
        <p><strong>Cargo:</strong> {{ ganador.Cargo }}</p>
        <p><strong>Empleado:</strong> {{ ganador.Nombre }} {{ ganador.Apellido }}</p>
      </div>
    </transition>
  </div>
</template>
<script setup>
import { ref, onMounted } from "vue";

const usuarios = ref([]);
const ganador = ref(null);
const girando = ref(false);
const itemHeight = 160;

const slots = ref([
  { title: "Departamento", items: [], position: 0, girando: false, velocidad: 8, activo: null },
  { title: "Cargo", items: [], position: 0, girando: false, velocidad: 8, activo: null },
  { title: "Empleado", items: [], position: 0, girando: false, velocidad: 8, activo: null },
]);

onMounted(async () => {
  const res = await fetch("/usuarios.json");
  usuarios.value = await res.json();
  slots.value[0].items = [...new Set(usuarios.value.map(u => u.Departamento))];
});

function iniciarSorteo() {
  if (girando.value) return;
  girando.value = true;
  ganador.value = null;

  // Reiniciar todo
  slots.value.forEach(s => {
    s.velocidad = 8;
    s.position = 0;
    s.girando = false;
    s.activo = null;
  });

  // Paso 1 → Girar el primer slot
  girarSlot(0).then(() => {
    // Cuando se detiene departamento, actualiza cargos y gira
    actualizarCargos();
    girarSlot(1).then(() => {
      // Cuando se detiene cargo, actualiza empleados y gira
      actualizarEmpleados();
      girarSlot(2).then(() => {
        // Cuando se detiene el último, calcula el ganador real
        finalizarGanador();
      });
    });
  });
}

function girarSlot(index) {
  return new Promise(resolve => {
    const slot = slots.value[index];
    if (!slot.items.length) return resolve();

    slot.girando = true;
    const total = slot.items.length * itemHeight;

    // 🔹 Elegimos el índice que quedará centrado
    const elegidoIndex = Math.floor(Math.random() * slot.items.length);

    // 🔹 Calculamos el desplazamiento para que ese ítem quede en el CENTRO
    const windowHeight = 480; // igual que en el CSS
    const offsetCentro = windowHeight / 2 - itemHeight / 2;
    const destino = -(elegidoIndex * itemHeight) + offsetCentro;

    // 🔹 Damos unas vueltas extra
    const inicio = slot.position;
    const distancia = destino - inicio - total * 4;
    const duracion = 3500 + index * 800;
    const t0 = performance.now();

    const easeOut = x => 1 - Math.pow(1 - x, 3);

    const animar = (t) => {
      const progreso = Math.min((t - t0) / duracion, 1);
      const eased = easeOut(progreso);
      slot.position = inicio + distancia * eased;

      if (progreso < 1) {
        requestAnimationFrame(animar);
      } else {
        // 🔸 Alinear al centro exacto
        slot.position = destino;
        slot.activo = slot.items[elegidoIndex];
        slot.girando = false;
        resolve();
      }
    };
    requestAnimationFrame(animar);
  });
}


function actualizarCargos() {
  const depto = slots.value[0].activo;
  if (!depto) return;

  // 🔹 Filtramos cargos válidos de ese departamento
  let cargos = [
    ...new Set(usuarios.value.filter(u => u.Departamento === depto).map(u => u.Cargo))
  ];

  // 🔸 Si tiene pocos (menos de 8), se repiten para llenar
  if (cargos.length > 0 && cargos.length < 8) {
    const repeticiones = Math.ceil(10 / cargos.length);
    cargos = Array.from({ length: repeticiones }, () => cargos).flat().slice(0, 10);
  }

  // 🔸 Si está vacío, usamos globales de emergencia
  if (!cargos.length) {
    const cargosGlobal = [...new Set(usuarios.value.map(u => u.Cargo))];
    cargos = cargosGlobal.slice(0, 10);
  }

  slots.value[1].items = cargos;
  slots.value[1].position = 0;
  slots.value[1].activo = null;
}

function actualizarEmpleados() {
  const depto = slots.value[0].activo;
  const cargo = slots.value[1].activo;
  if (!depto || !cargo) return;

  // 🔹 Filtramos empleados válidos
  let empleados = usuarios.value
    .filter(u => u.Departamento === depto && u.Cargo === cargo)
    .map(u => `${u.Nombre} ${u.Apellido}`);

  // 🔸 Si tiene pocos, se repiten hasta completar 10
  if (empleados.length > 0 && empleados.length < 8) {
    const repeticiones = Math.ceil(10 / empleados.length);
    empleados = Array.from({ length: repeticiones }, () => empleados).flat().slice(0, 10);
  }

  // 🔸 Si no hay ninguno, tomamos globales
  if (!empleados.length) {
    const randoms = usuarios.value
      .slice(0, 10)
      .map(u => `${u.Nombre} ${u.Apellido}`);
    empleados = randoms;
  }

  slots.value[2].items = empleados;
  slots.value[2].position = 0;
  slots.value[2].activo = null;
}

function finalizarGanador() {
  const depto = slots.value[0].activo;
  const cargo = slots.value[1].activo;
  const nombreCompleto = slots.value[2].activo;
  if (!depto || !cargo || !nombreCompleto) return;

  const [nombre, apellido] = nombreCompleto.split(" ");
  const persona = usuarios.value.find(
    u =>
      u.Departamento === depto &&
      u.Cargo === cargo &&
      u.Nombre === nombre &&
      u.Apellido.startsWith(apellido)
  );

  ganador.value = persona || { Departamento: depto, Cargo: cargo, Nombre: nombre, Apellido: apellido };
  girando.value = false;
}
</script>



<style scoped>
.slot-machine {
  text-align: center;
  padding: 2rem;
  font-family: "Segoe UI", sans-serif;
  background: radial-gradient(circle at center, #f9f9f9, #e6f0ff);
  min-height: 100vh;
}

.title {
  margin-bottom: 1rem;
  color: #003870;
  font-weight: 700;
  font-size: 1.8rem;
}

.btn {
  background-color: #b80000;
  color: white;
  border: none;
  border-radius: 10px;
  padding: 12px 25px;
  cursor: pointer;
  font-weight: 600;
  font-size: 1.1rem;
  margin-bottom: 1.5rem;
  box-shadow: 0 0 12px rgba(255, 0, 0, 0.5);
  transition: all 0.2s ease-in-out;
}
.btn:hover {
  background-color: #ff2222;
  transform: scale(1.05);
}

.slots {
  display: flex;
  justify-content: center;
  gap: 2rem;
}

.slot {
  text-align: center;
  position: relative;
}

.slot-window {
  position: relative;
  background: linear-gradient(180deg, #003366, #004b8a);
  border-radius: 20px;
  width: 300px;
  height: 480px;
  overflow: hidden;
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.45);
  border: 3px solid #00baff60;
}

/* 🔹 Faja roja ahora va ATRÁS del texto */
.highlight-line {
  position: absolute;
  top: 50%;
  left: 0;
  width: 100%;
  height: 160px;
  background: linear-gradient(90deg, #b80000, #ff4747, #b80000);
  transform: translateY(-50%);
  z-index: 0; /* 🔸 ahora va detrás */
  box-shadow: 0 0 35px rgba(255, 0, 0, 0.5);
  border-top: 2px solid #ffbcbc;
  border-bottom: 2px solid #ffbcbc;
  opacity: 0.9;
  animation: glow 1.5s infinite alternate;
}
@keyframes glow {
  from { box-shadow: 0 0 25px rgba(255, 0, 0, 0.4); }
  to { box-shadow: 0 0 45px rgba(255, 0, 0, 0.9); }
}

.slot-wrap {
  position: relative;
  z-index: 2; /* 🔹 el texto ahora por encima de la faja */
  will-change: transform;
}

.slot-item {
  height: 160px;
  line-height: 160px;
  color: #fff; /* 🔹 texto siempre blanco visible */
  font-weight: 600;
  font-size: 1.1rem;
  text-align: center;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  opacity: 0.95;
  text-shadow: 0 0 8px rgba(0, 0, 0, 0.4);
  background: rgba(0, 0, 0, 0.05);
  user-select: none;
  transition: opacity 0.4s ease, transform 0.3s ease;
}

/* 🎁 Tarjeta del ganador */
.winner-card {
  margin-top: 2rem;
  background: white;
  border-radius: 16px;
  padding: 1.5rem 2rem;
  display: inline-block;
  text-align: left;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.25);
  animation: pop 0.6s ease-out;
}

@keyframes pop {
  from { transform: scale(0.85); opacity: 0; }
  to { transform: scale(1); opacity: 1; }
}
</style>

