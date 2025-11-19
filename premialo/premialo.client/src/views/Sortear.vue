<template>

 <!--Botón Volver al Panel de Sorteos -->
    <div class="btn-volver-container">
      <button class="btn-flecha" @click="volverASorteos">
        <i class="pi pi-arrow-left"></i>
      </button>
    </div>

    <div ref="containerRef" class="container">
      <!-- Área de animación -->
      <div class="animation-area">
        <!-- Copos en espiral -->
        <div v-for="(copo, index) in coposEspiral" :key="'espiral-' + index" class="snowflake" :style="{
          position: 'absolute',
          left: copo.x + 'px',
          top: copo.y + 'px',
          transform: `translate(-50%, -50%) rotate(${copo.rotacion}deg) scale(${copo.escala})`,
          opacity: copo.opacidad,
          zIndex: copo.zIndex
        }">
          <img src="../assets/copo-de-nieve.png" alt="" class="snowflake-icon">
        </div>

        <!-- Copo central (pop final) -->
        <div v-if="copoPosition && copoOpacity > 0" class="snowflake" :style="{
          position: 'absolute',
          left: copoPosition.x + 'px',
          top: copoPosition.y + 'px',
          transform: `translate(-50%, -50%) scale(${copoScale})`,
          opacity: copoOpacity,
          zIndex: 20
        }">
          <img src="../assets/copo-de-nieve.png" alt="" class="snowflake-icon">
        </div>

        <!-- Contador regresivo -->
        <div v-if="estado === 'animando' && contador !== null" :key="contador" class="counter">
          {{ contador }}
        </div>

        <!-- Tarjeta premium del ganador -->
        <div v-if="false" class="winner-card">
          <div class="gold-card" ref="goldCardRef" @mousemove="handleTilt" @mouseleave="resetTilt">
            <div class="shine"></div>
            <div class="micro"></div>

            <div class="content">
              <div class="top-label">¡GANADOR DEL SORTEO!</div>
              <div class="amount">RD${{ formatAmount(ganador.premio) }}</div>
              <div class="desc">{{ amountToWords(ganador.premio) }} PESOS DOMINICANOS</div>

              <div class="bank">CONTRALORIA GENERAL</div>

              <div class="serial">Serial: {{ generateSerial() }}</div>

              <div class="winner">{{ ganador.nombre }}</div>
            </div>
          </div>
        </div>

        <div v-if="estado === 'finalizado' && resultado">
          <div class="gold-card" ref="goldCardRef" @mousemove="handleTilt" @mouseleave="resetTilt">
            <div class="shine"></div>
            <div class="micro"></div>

            <div class="content">
              <div class="top-label">¡GANADOR DEL SORTEO!</div>
              <div class="amount">{{ proximoPremio.nombre }}</div>

              <div class="bank">CONTRALORIA GENERAL</div>

              <div class="serial">{{ resultado.participante.unidad }}</div>
              <div class="serial">{{ resultado.participante.cargo }}</div>
              <div class="winner">{{ resultado.participante.nombres }} {{ resultado.participante.apellidos }}</div>
            </div>
          </div>
        </div>

        <!-- Estado inicial -->
        <div v-if="estado === 'inicial'" class="initial-state">
          <img src="../assets/copo-de-nieve.png" alt="" class="initial-icon">
          <h1 class="title">
            {{sorteoActual.nombre}}
          </h1>
          <h3 class="text-white">Siguiente premio:</h3>
          <h5 class="text-white">{{proximoPremio.nombre}}</h5>
        </div>
      </div>

      <!-- Controles -->
      <div class="controls" v-if="hayPremios">
        <button v-if="estado === 'inicial'" @click="iniciarSorteo" class="btn btn-primary">
          <svg class="btn-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <polygon points="6 3 20 12 6 21 6 3" />
          </svg>
          Iniciar Sorteo
        </button>

        <button v-if="estado === 'finalizado'" @click="reiniciarSorteo" class="btn btn-secondary">
          <svg class="btn-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path d="M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8" />
            <path d="M3 3v5h5" />
          </svg>
          Siguiente Sorteo
        </button>
      </div>

      <!-- Decoración de fondo -->
      <div class="background-decoration">
        <div class="decoration-circle circle-1" />
        <div class="decoration-circle circle-2" />
        <div class="decoration-circle circle-3" />
      </div>
    </div>
</template>

<script>
import confetti from 'canvas-confetti'
import api from "@/utilities/api";

// Datos de ejemplo para ganadores
const participantes = [
  { nombre: 'María García', foto: 'https://i.pravatar.cc/150?img=1', codigo: 'P-001', premio: 10000 },
  { nombre: 'Carlos Rodríguez', foto: 'https://i.pravatar.cc/150?img=2', codigo: 'P-002', premio: 10000 },
  { nombre: 'Ana Martínez', foto: 'https://i.pravatar.cc/150?img=3', codigo: 'P-003', premio: 10000 },
  { nombre: 'José López', foto: 'https://i.pravatar.cc/150?img=4', codigo: 'P-004', premio: 10000 },
  { nombre: 'Laura Fernández', foto: 'https://i.pravatar.cc/150?img=5', codigo: 'P-005', premio: 10000 },
]

// Función easeOutCubic
const easeOutCubic = (t) => --t * t * t + 1

export default {
  name: "Sortear",

  data() {
    return {
      hayPremios: false,
      sorteoActual: {
        nombre: "Cargando...",
        descripcion: ""
      },
      proximoPremio: {
        nombre: "Cargando...",
        descripcion: ""
      },
      resultado: {
        premio: {
          nombre: "",
          descripcion: ""
        },
        participante: {
          nombres: "",
          apellidos: "",
          documentoIdentidad: "",
          cargo: "",
          unidad: ""
        }
      },
      estado: 'inicial',
      ganador: null,
      contador: null,

      copoPosition: null,
      copoScale: 1,
      copoOpacity: 0,
      copoRotation: 0,

      coposEspiral: [],
      animacionFrame: null,

      cx: 0,
      cy: 0,
    }
  },

  async mounted() {
    if (this.$refs.containerRef) {
      const rect = this.$refs.containerRef.getBoundingClientRect()
      this.cx = rect.width / 2
      this.cy = rect.height / 2
    }

    await this.loadSorteoActual();
    await this.loadNextPremio();
  },

  methods: {
    async loadSorteoActual() {
       const id = this.$route.params.id;

        try {
          const response = await api.get(`/sorteos/${id}/single`);
          this.sorteoActual = response.data;
       } catch(e) {
          
          if(e.status == 404) {
            this.sorteoActual = { nombre: e.response.data };
          }
       }
    },
    async loadNextPremio() {
        const id = this.$route.params.id;

       try {
          const response = await api.get(`/sorteos/${id}/proximo-premio`);
          this.proximoPremio = response.data;
          this.hayPremios = true;
       } catch(e) {
          
          if(e.status == 404) {
            this.hayPremios = false;
            this.proximoPremio = { nombre: e.response.data };
          }
       }

    },

    async getGanador() {
        const id = this.$route.params.id;

       try {
          const response = await api.post(`/sorteos/${id}/sortear`);
          this.resultado = response.data;
       } catch(e) {
          
          if(e.status == 404) {
            this.proximoPremio = { nombre: e.response.data };
          }
       }
    },

    volverASorteos() {
      const id = this.$route.params.id
      this.$router.push("/sorteos/" + id);
    },

    lanzarConfeti() {
      const duracion = 4000
      const fin = Date.now() + duracion
      const config = {
        startVelocity: 30,
        spread: 360,
        ticks: 60,
        zIndex: 100,
        particleCount: 150
      }

      const random = (min, max) => Math.random() * (max - min) + min

      const intervalo = setInterval(() => {
        if (Date.now() >= fin) {
          clearInterval(intervalo)
          return
        }

        confetti({
          ...config,
          origin: { x: random(0.1, 0.3), y: Math.random() - 0.2 },
          colors: ['#FFFFFF', '#60A5FA']
        })

        confetti({
          ...config,
          origin: { x: random(0.7, 0.9), y: Math.random() - 0.2 },
          colors: ['#FFFFFF', '#93C5FD']
        })
      }, 250)
    },

    resetearConfeti() {
      confetti.reset()
    },

    async iniciarSorteo() {
      if (this.estado !== 'inicial') return

      this.estado = 'animando'
      this.contador = 3
      this.copoOpacity = 0

      // Seleccionar ganador
      //this.ganador = participantes[Math.floor(Math.random() * participantes.length)]

      await this.getGanador();

      await new Promise((r) => setTimeout(r, 100))

      if (!this.$refs.containerRef) return

      const rect = this.$refs.containerRef.getBoundingClientRect()
      this.cx = rect.width / 2
      this.cy = rect.height / 2

      const startX = rect.width
      const startY = 0
      const radioInicial = Math.sqrt((this.cx - startX) ** 2 + (this.cy - startY) ** 2)

      const duracionEspiral = 4000
      const startTime = Date.now()

      const animarEspiral = () => {
        const elapsed = Date.now() - startTime
        const progresoLineal = Math.min(elapsed / duracionEspiral, 1)
        const progreso = easeOutCubic(progresoLineal)

        const vueltas = 3
        const numCopos = 15
        const offset = 0.03

        if (progresoLineal < 1) {
          const nuevos = []

          for (let i = 0; i < numCopos; i++) {
            const p = progreso - i * offset
            if (p < 0) continue

            const angulo = p * vueltas * 2 * Math.PI - Math.PI / 4
            const radioActual = (1 - p) * radioInicial

            nuevos.push({
              x: this.cx + Math.cos(angulo) * radioActual,
              y: this.cy + Math.sin(angulo) * radioActual,
              rotacion: p * 360 * 5,
              escala: (0.2 + p * 0.8) * (1 - (i / numCopos) * 0.5),
              opacidad: 1 - (i / numCopos) * 0.7,
              zIndex: 20
            })
          }

          this.coposEspiral = nuevos

          if (progreso < 0.25) this.contador = 3
          else if (progreso < 0.5) this.contador = 2
          else if (progreso < 0.75) this.contador = 1
          else this.contador = 0

          this.animacionFrame = requestAnimationFrame(animarEspiral)
        } else {
          this.coposEspiral = []
          this.contador = null
          this.expandirCopo()
        }
      }

      this.animacionFrame = requestAnimationFrame(animarEspiral)
    },

    expandirCopo() {
      const secuencia = [1.2, 0.9, 1.4, 0.95, 1.6, 1.0, 2.5, 3.0]
      let paso = 0

      this.copoPosition = { x: this.cx, y: this.cy }
      this.copoScale = 1
      this.copoOpacity = 1

      const intervaloPop = setInterval(() => {
        if (paso < secuencia.length) {
          this.copoScale = secuencia[paso]
          paso++
        } else {
          clearInterval(intervaloPop)

          this.copoScale = secuencia[secuencia.length - 1]

          const fadeStart = Date.now()
          const fadeDuration = 800

          const fadeOut = () => {
            const elapsed = Date.now() - fadeStart
            const p = Math.min(elapsed / fadeDuration, 1)

            this.copoOpacity = 1 - p

            if (p < 1) requestAnimationFrame(fadeOut)
            else {
              this.lanzarConfeti()
              setTimeout(() => {
                this.estado = 'finalizado'
                this.copoPosition = null
              }, 500)
            }
          }

          fadeOut()
        }
      }, 120)
    },

    reiniciarSorteo() {
      if (this.animacionFrame) cancelAnimationFrame(this.animacionFrame)

      this.estado = 'inicial'
      this.ganador = null
      this.contador = null

      this.copoPosition = null
      this.copoRotation = 0
      this.copoScale = 1
      this.copoOpacity = 0
      this.coposEspiral = []

      this.resetearConfeti();

      this.loadNextPremio();
    },

    getInitials(nombre) {
      return nombre.split(' ').map(x => x[0]).join('').toUpperCase()
    },

    amountToWords(monto) {
      if (monto === 10000) return 'DIEZ MIL'
      if (monto === 50000) return 'CINCUENTA MIL'
      if (monto === 100000) return 'CIEN MIL'
      return 'DIEZ MIL'
    },

    generateSerial() {
      const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789'
      let serial = ''
      for (let i = 0; i < 12; i++) {
        serial += chars.charAt(Math.floor(Math.random() * chars.length))
      }
      return serial
    },

    formatAmount(monto) {
      return monto >= 1000 ? (monto / 1000) + 'K' : monto
    },

    formatCurrency(monto) {
      return new Intl.NumberFormat('es-DO', {
        style: 'currency',
        currency: 'DOP'
      }).format(monto)
    },

    handleTilt(e) {
      const card = this.$refs.goldCardRef
      if (!card) return

      const threshold = 75
      const maxTilt = 10

      const rect = card.getBoundingClientRect()
      const x = e.clientX - rect.left
      const y = e.clientY - rect.top

      let tiltX = 0
      let tiltY = 0

      if (x < threshold && y < threshold) {
        tiltY = maxTilt
        tiltX = -maxTilt
      } else if (x > rect.width - threshold && y < threshold) {
        tiltY = -maxTilt
        tiltX = -maxTilt
      } else if (x < threshold && y > rect.height - threshold) {
        tiltY = maxTilt
        tiltX = maxTilt
      } else if (x > rect.width - threshold && y > rect.height - threshold) {
        tiltY = -maxTilt
        tiltX = maxTilt
      }

      card.style.transform = `perspective(1500px) rotateX(${tiltX}deg) rotateY(${tiltY}deg) scale(1.02)`
    },

    resetTilt() {
      const card = this.$refs.goldCardRef
      if (!card) return
      card.style.transform = `perspective(1500px) rotateX(0deg) rotateY(0deg) scale(1)`
    },
  }
}
</script>


<style scoped>

/* Estilos para la tarjeta premium */

.gold-card {
  width: var(--card-w);
  height: var(--card-h);
  border-radius: 18px;
  position: relative;
  overflow: hidden;
  cursor: default;
  transform-style: preserve-3d;

  background: linear-gradient(135deg,
    var(--gold-1),
    var(--gold-2) 30%,
    var(--gold-3) 70%,
    var(--gold-4)
  );

  /* MARCO DORADO PREMIUM */
  border: 2.5px solid rgba(255,230,130,0.8);

  /* NEÓN PERMANENTE */
  box-shadow: var(--neon-gold);

  /* Glow interno + brillo */
  box-shadow: var(--neon-gold), inset 0 0 20px rgba(255,255,255,0.2);

  /* Transición de inclinación más suave */
  transition: transform 0.4s ease-out;
}

/* TEXTURA DE ORO REAL */
.gold-card::before {
  content:"";
  position:absolute;
  inset:0;
  background: repeating-linear-gradient(
    115deg,
    rgba(255,255,255,0.16) 0%,
    rgba(255,255,255,0.03) 7%,
    rgba(0,0,0,0.2) 11%
  );
  mix-blend-mode: overlay;
  opacity: 0.7;
  pointer-events: none;
}

/* BRILLO / SHINE */
.shine {
  position: absolute;
  inset: -60%;
  transform: rotate(-35deg);
  background: linear-gradient(
    90deg,
    rgba(255,255,255,0) 0%,
    rgba(255,255,255,0.6) 50%,
    rgba(255,255,255,0) 100%
  );
  filter: blur(14px);
  animation: shineMove 3.2s infinite;
  pointer-events: none;
}

@keyframes shineMove {
  0% { transform: translateX(-150%) rotate(-35deg); opacity:0; }
  25% { opacity:0.8; }
  50% { transform: translateX(130%) rotate(-35deg); opacity:0.6; }
  100% { transform: translateX(260%) rotate(-35deg); opacity:0; }
}

/* MICROTEXTURA SEGURIDAD */
.micro {
  position:absolute;
  inset:0;
  background-image: repeating-linear-gradient(
    0deg,
    rgba(255,255,255,0.05) 0,
    rgba(255,255,255,0.05) 1px,
    transparent 1px,
    transparent 4px
  );
  mix-blend-mode: soft-light;
  pointer-events:none;
}

/* CONTENIDO */
.content {
  position:absolute;
  inset:0;
  padding:30px;
  color:#3c2f0a;
  z-index:10;
  text-shadow: 0 1px 2px rgba(255,255,255,0.45);
}

.top-label {
  font-size: 18px;
  font-weight: 800;
  letter-spacing: 2px;
  margin-bottom: 10px;
}

.amount {
  font-size: 55px;
  font-weight: 900;
  margin-top: 10px;
}

.desc {
  margin-top: 5px;
  font-size: 20px;
  font-weight: 600;
}

.bank {
  margin-top: 18px;
  font-size: 14px;
  font-weight: 600;
  opacity: 0.8;
}

.serial {
  margin-top: 12px;
  font-size: 14px;
  font-weight: 600;
}

.winner {
  position:absolute;
  bottom: 20px;
  right: 25px;
  font-size: 30px;
  font-weight: 800;
  text-align:right;
  /* Estilos ajustados para que el nombre resalte más */
  color: #1a1a1a;
  text-shadow: 1px 1px 2px var(--gold-4);
}

/* Estilos existentes */
.snowflake {
  transition: opacity 0.5s ease, transform 0.2s ease;
  pointer-events: none;
}

.snowflake-icon {
  width: 5rem;
  height: 5rem;
  filter: drop-shadow(0 0 15px rgba(255, 255, 255, 0.8));
}

.counter {
  position: absolute;
  color: rgba(255, 255, 255, 0.8);
  pointer-events: none;
  font-size: 12rem;
  font-weight: bold;
  text-shadow: 0 0 40px rgba(96, 165, 250, 0.8);
  line-height: 1;
  animation: counterPop 0.4s ease-out;
}

@keyframes counterPop {
  0% {
    transform: scale(0.3);
    opacity: 0;
  }

  60% {
    transform: scale(1.3);
    opacity: 1;
  }

  100% {
    transform: scale(1);
    opacity: 1;
  }
}

/* Estilos base */
.min-h-screen {
  min-height: 100vh;
}

.flex {
  display: flex;
}

.items-center {
  align-items: center;
}

.justify-center {
  justify-content: center;
}

.p-4 {
  padding: 1rem;
}

.overflow-hidden {
  overflow: hidden;
}

/* Contenedor principal */
.container {
  position: relative;
  width: 100%;
  height: 100%;
  max-width: 72rem;
  max-height: 90vh;
  aspect-ratio: 16 / 9;
  background-color: transparent;
  backdrop-filter: blur(4px);
  border-radius: 1.5rem;
}

/* Área de animación */
.animation-area {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Tarjeta del ganador */
.winner-card {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
}

/* Estado inicial */
.initial-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.5rem;
  z-index: 10;
  animation: initialAppear 0.5s ease-out;
}

@keyframes initialAppear {
  0% {
    opacity: 0;
    transform: translateY(20px);
  }

  100% {
    opacity: 1;
    transform: translateY(0);
  }
}

.initial-icon {
  width: 6rem;
  height: 6rem;
  color: rgb(147, 197, 253);
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

.title {
  color: white;
  text-align: center;
  font-size: 2.25rem;
  font-weight: bold;
}

.subtitle {
  color: rgb(191, 219, 254);
  text-align: center;
  max-width: 28rem;
  font-size: 1.125rem;
}

/* Controles */
.controls {
  position: absolute;
  bottom: 2rem;
  left: 50%;
  transform: translateX(-50%);
  z-index: 20;
}

.btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 2rem;
  font-size: 1.125rem;
  font-weight: 500;
  border-radius: 0.5rem;
  transition: all 0.3s;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
}

.btn:hover {
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
}

.btn-primary {
  background-color: rgb(59, 130, 246);
  color: white;
}

.btn-primary:hover {
  background-color: rgb(37, 99, 235);
}

.btn-secondary {
  background-color: rgba(255, 255, 255, 0.9);
  color: rgb(29, 78, 216);
  border: 1px solid rgb(147, 197, 253);
}

.btn-secondary:hover {
  background-color: rgb(255, 255, 255);
}

.btn-icon {
  width: 1.5rem;
  height: 1.5rem;
}

/* Decoración de fondo */
.background-decoration {
  position: absolute;
  inset: 0;
  pointer-events: none;
}

.decoration-circle {
  position: absolute;
  border-radius: 9999px;
  filter: blur(64px);
}

.circle-1 {
  top: 2.5rem;
  left: 2.5rem;
  width: 8rem;
  height: 8rem;
  background-color: rgba(96, 165, 250, 0.1);
}

.circle-2 {
  bottom: 2.5rem;
  right: 2.5rem;
  width: 10rem;
  height: 10rem;
  background-color: rgba(59, 130, 246, 0.1);
}

.circle-3 {
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 16rem;
  height: 16rem;
  background-color: rgba(255, 255, 255, 0.05);
}

/* 🔹 Contenedor para posicionamiento */
.btn-volver-container {
  position: relative;
  display: flex;
  justify-content: flex-start;
  padding-left: 1.2rem;
  margin-top: 1.2rem;
  /* separación del header */
}

/* 🔹 Botón estilo flecha */
.btn-flecha {
  background: linear-gradient(145deg, #082b57, #0b3a70);
  color: #ffffff;
  border: none;
  border-radius: 0.75rem;
  width: 62px;
  height: 62px;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 1.2rem;
  cursor: pointer;
  box-shadow: 0 2px 4px rgba(0, 80, 150, 0.3);
  transition: all 0.3s ease;
}

/* Hover elegante con glow */
.btn-flecha:hover {
  background: linear-gradient(145deg, #0d4584, #0a5ca8);
  box-shadow: 0 0 12px rgba(0, 145, 255, 0.4);
  transform: translateY(-2px);
}


/* Animaciones */
@keyframes bounce {

  0%,
  20%,
  50%,
  80%,
  100% {
    transform: translateY(0);
  }

  40% {
    transform: translateY(-10px);
  }

  60% {
    transform: translateY(-5px);
  }
}

@keyframes pulse {

  0%,
  100% {
    opacity: 1;
  }

  50% {
    opacity: 0.5;
  }
}
</style>
