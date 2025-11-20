<template>
  <div class="admin-view container-fluid py-2 animate__animated animate__fadeIn">

    <!-- 🔹 HEADER -->
    <!-- <Header :perfilUsuario="perfilUsuario" @cerrar-sesion="cerrarSesion" /> -->

    <!-- 🔹 Botón Volver -->
    <!-- <div class="btn-volver-container">
      <button class="btn-flecha" @click="volverASorteos">
        <i class="pi pi-arrow-left"></i>
      </button>
    </div> -->


    <!-- 🔹 Botón Ir al Sorteo -->
    <!-- <div class="container d-flex justify-content-end">
      <button class="btn btn-lg btn-success fw-bold px-5 py-3 mb-4 pulse-wide"
        @click="$router.push(`/sortear/${$route.params.id}`)">
        Ir al Sorteo
      </button>
    </div> -->

    <div class="container d-flex justify-content-around align-items-center">
      <!-- 🔹 Encabezado dinámico del sorteo -->
      <div class="encabezado-sorteo text-center mb-4 animate__animated animate__fadeInDown">
        <h3 class="fw-bold text-light mb-1">
          {{ sorteo?.nombre || "Sorteo sin nombre" }}
        </h3>

        <p class="text-white-50 mb-0">
          Fecha del sorteo:
          <span class="fw-semibold text-info">
            {{ formatearFecha(sorteo?.fechaSorteo) }}
          </span>
        </p>
      </div>

      <!-- 🔹 Botón Ir al Sorteo -->
      <div class="d-flex justify-content-end">
        <button class="btn btn-lg btn-success fw-bold px-5 py-3 mb-4 pulse-wide"
          @click="$router.push(`/sortear/${$route.params.id}`)">
          Ir al Sorteo
        </button>
      </div>

    </div>



    <!-- ===========================
           CONTENIDO PRINCIPAL
    ============================ -->
    <main class="container">


      <div class="row g-4">


        <!-- 🔹 Registro de Asistencia -->
        <div class="col-12 col-md-6 col-lg-4 container-tarjetas">
          <RegistroAsistencia :totalConcursando="totalConcursando" :totalAsistencia="totalAsistencia"
            :sorteoId="$route.params.id" @registrar-asistencia="modalAsistencia = true" />
        </div>


        <!-- 🔹 Gestión de Participantes -->
        <div class="col-12 col-md-6 col-lg-4 container-tarjetas">
          <ParticipantesCard :totalParticipantes="totalParticipantes" :totalConcursando="totalConcursando"
            @ver-participantes="verParticipantes" @eliminar-base="eliminarBase" />
        </div>

        <!-- 🔹 Gestión de Premios (solo Admin) -->
        <div v-if="esAdmin" class="col-12 col-md-6 col-lg-4 container-tarjetas">
          <PremiosCard :totalPremios="totalPremios" :premiosPendientes="premiosPendientes"
            :premiosSorteados="premiosSorteados" @ver-premios="mostrarModalPremios = true" />
        </div>


        <!-- Tabla de Resultados -->
        <div class="col-12 col-md-12 col-lg-12">
          <ResultadosCard :resultados="resultados" @ver-participantes="verParticipantes" @eliminar-base="eliminarBase"
            @descargar-xls="exportarGanadores" />
        </div>


        <!-- ===========================
                MODALES
        ============================ -->

        <!-- Modal Participantes -->
        <ModalParticipantes :visible="mostrarModal" :sorteoId="$route.params.id" @cerrar="onCerrarModalParticipantes" />

        <!-- Modal Premios -->
        <ModalPremios :visible="mostrarModalPremios" :sorteoId="$route.params.id" @cerrar="onCerrarModalPremios" />

        <!-- Modal asistencia -->
        <ModalAsistencia :visible="modalAsistencia" :sorteoId="$route.params.id" @cerrar="modalAsistencia = false"
          @registrado="cargarDatosSorteo" />
      </div>


    </main>

  </div>
</template>


<script>
import ParticipantesCard from '@/components/ParticipantesCard.vue';
import ModalParticipantes from '@/components/ModalParticipantes.vue';
import RegistroAsistencia from '@/components/RegistroAsistencia.vue';
import ResultadosCard from '@/components/ResultadosCard.vue';
import PremiosCard from '@/components/PremiosCard.vue';
import ModalPremios from '@/components/ModalPremios.vue';
import ModalAsistencia from '@/components/ModalAsistencia.vue';

import Header from '@/components/Header.vue';
import api from "@/utilities/api";

import * as XLSX from "xlsx";


export default {
  name: "AdminView",
  components: {
    ParticipantesCard,
    ModalParticipantes,
    ModalAsistencia,
    RegistroAsistencia,
    ResultadosCard,
    ModalPremios,
    PremiosCard,
    Header
  },
  data() {
    return {
      menuVisible: false,
      mostrarModal: false,
      modalAsistencia: false,
      totalParticipantes: 0,
      totalAsistencia: 0,
      totalConcursando: 0,

      mostrarModalPremios: false,
      totalPremios: 0,
      premiosPendientes: 0,
      premiosSorteados: 0,
      resultados: [],
      sorteo: null,
      perfilUsuario: localStorage.getItem("perfil") || "Administrador",
    };
  },
  computed: {
    esAdmin() {
      return this.perfilUsuario === "Administrador";
    },
    esCoordinador() {
      return this.perfilUsuario === "Coordinador";
    },
  },
  methods: {
    toggleMenu() {
      this.menuVisible = !this.menuVisible;
    },
    volverASorteos() {
      this.$router.push("/sorteos");
    },
    cerrarSesion() {
      localStorage.removeItem("perfil");
      this.$router.push("/login");
    },
    verParticipantes() {
      this.mostrarModal = true;
    },
    eliminarBase() {
      this.totalParticipantes = 0;
      this.totalActivos = 0;
    },
    onCerrarModalParticipantes() {
      this.mostrarModal = false;
      this.cargarDatosSorteo();
    },
    onCerrarModalPremios() {
      this.mostrarModalPremios = false;
      this.cargarDatosSorteo();
    },
    formatearFecha(fecha) {
      if (!fecha) return "";
      const f = new Date(fecha);
      return f.toLocaleDateString("es-DO", {
        year: "numeric",
        month: "long",
        day: "numeric",
      });
    },
    exportarGanadores() {
      try {
        if (!this.resultados || this.resultados.length === 0) {
          this.$toast?.add({
            severity: "info",
            summary: "Sin datos",
            detail: "No hay ganadores para exportar.",
            life: 3500,
          });
          return;
        }

        // Crear datos limpios para exportar
        const data = this.resultados.map((r, index) => ({
          "#": index + 1,
          "Premio": r.premio.nombre,
          "Ganador": `${r.participante.nombres} ${r.participante.apellidos}`,
          "Documento": r.participante.documentoIdentidad || "",
          "Cargo": r.participante.cargo || ""
        }));

        // Convertir a hoja de Excel
        const worksheet = XLSX.utils.json_to_sheet(data);

        // Crear libro Excel
        const workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, "Ganadores");

        // Nombre dinámico del archivo
        const nombreArchivo = `ganadores_sorteo_${this.sorteo?.nombre || 'evento'}.xlsx`
          .replace(/\s+/g, "_")
          .toLowerCase();

        // Descargar archivo
        XLSX.writeFile(workbook, nombreArchivo);

        this.$toast?.add({
          severity: "success",
          summary: "Descarga completada",
          detail: "El archivo XLS se ha descargado correctamente.",
          life: 3500,
        });

      } catch (error) {
        console.error("Error al exportar XLS:", error);

        this.$toast?.add({
          severity: "error",
          summary: "Error al descargar",
          detail: "Hubo un problema al generar el archivo XLS.",
          life: 3500,
        });
      }
    },
    async cargarDatosSorteo() {
      try {
        const id = this.$route.params.id;

        const response = await api.get(`/sorteos/${id}`);
        this.sorteo = response.data;

        console.log(this.sorteo)
        // this.totalParticipantes = this.sorteo.participantes.length;
        // this.totalConcursando = this.sorteo.participantes.filter(p => p.estatus === 'Concursando').length;

        this.totalParticipantes = this.sorteo.totalParticipantes;
        this.totalConcursando = this.sorteo.totalParticipantesAsistieron;

        this.totalPremios = this.sorteo.totalPremios;
        this.premiosPendientes = this.sorteo.premiosPendientes;
        this.premiosSorteados = this.sorteo.premiosSorteados;

        this.totalAsistencia = this.sorteo.totalAsistencias;

        this.resultados = this.sorteo.resultados || [];

      } catch (error) {
        console.error("Error al cargar el sorteo:", error);
        this.$router.push("/sorteos");
      }
    }
  },
  watch: {
    '$route.params.id': {
      immediate: false,
      handler() {
        this.cargarDatosSorteo();
      }
    }
  }, mounted() {
    // 🔹 Entrar directamente sin login
    this.perfilUsuario = "Administrador";
    this.sorteo = {
      id: this.$route.params.id,
      nombre: "Sorteo de Prueba Local",
      fechaSorteo: "2025-12-20T18:00:00"
    };
  }

  // mounted() {
  //   const perfil = localStorage.getItem("perfil");
  //   if (!perfil) {
  //     this.$router.push("/login");
  //   } else {
  //     this.perfilUsuario = perfil;
  //     this.cargarDatosSorteo();
  //   }
  // },
};
</script>


<style scoped>
.admin-view {
  position: relative;
  min-height: 100vh;
  color: #fff;
  z-index: 1;
}

/* header */
.admin-header {
  position: sticky;
  top: 0;
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(8px);
  border-radius: 0.75rem;
  z-index: 100;
}

.logo-bg {
  background: rgba(255, 255, 255, 0.85);
  border-radius: 1rem;
  padding: 0.4rem 0.8rem;
}

.logo-header {
  width: 80px;
  height: auto;
}

/*Botón rectangular del usuario */
.user-chip {
  background: linear-gradient(135deg, #0058b0, #003b84);
  color: #fff;
  border: none;
  padding: 0.55rem 1.3rem;
  border-radius: 1.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 0 10px rgba(0, 91, 196, 0.4);
}

.user-chip:hover {
  background: linear-gradient(135deg, #0066cc, #004a99);
  box-shadow: 0 0 18px rgba(0, 123, 255, 0.7);
  transform: translateY(-1px);
}

.encabezado-sorteo h3 {
  color: #bce6ff;
  text-shadow: 0 0 10px rgba(0, 150, 255, 0.3);
}

.encabezado-sorteo span {
  text-transform: capitalize;
}

/*Dropdown elegante */
.dropdown-menu-custom {
  position: absolute;
  right: 0;
  top: 115%;
  background: rgba(0, 60, 130, 0.35);
  backdrop-filter: blur(12px);
  border-radius: 0.75rem;
  padding: 0.4rem;
  min-width: 160px;
  z-index: 200;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.25);
  animation: fadePop 0.25s ease forwards;
}

/*Botón dentro del menú */
.dropdown-item-custom {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  width: 100%;
  color: #ffffff;
  background: transparent;
  border: none;
  padding: 0.6rem 0.8rem;
  border-radius: 0.5rem;
  text-align: center;
  font-weight: 500;
  transition: all 0.25s ease;
}

.dropdown-item-custom:hover {
  background: rgba(255, 255, 255, 0.15);
  color: #ff4b4b;
}

/* Nueva animación tipo “fade + pop” */
@keyframes fadePop {
  0% {
    opacity: 0;
    transform: scale(0.9);
  }

  100% {
    opacity: 1;
    transform: scale(1);
  }
}


/* Tarjetas */
.admin-card {
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(8px);
  border-radius: 1rem;
  padding: 1.5rem;
  box-shadow: 0 3px 12px rgba(0, 0, 0, 0.2);
  transition: all 0.3s ease;
}

.admin-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 4px 15px rgba(255, 255, 255, 0.1);
}

/* Títulos */
.card-title {
  font-weight: 700;
  color: #5cc4ff;
}

/* Inputs */
.form-control {
  background-color: rgba(255, 255, 255, 0.9);
  border: none;
  border-radius: 0.5rem;
}

/*Botón animado IR AL SORTEO */
.pulse-wide {
  animation: pulse-wide 1.8s infinite;
  width: 320px;
  font-size: 1.3rem;
}

@keyframes pulse-wide {
  0% {
    transform: scale(1);
    box-shadow: 0 0 0 0 rgba(25, 135, 84, 0.7);
  }

  70% {
    transform: scale(1.07);
    box-shadow: 0 0 0 25px rgba(25, 135, 84, 0);
  }

  100% {
    transform: scale(1);
  }
}

/* Transición menú */
.fade-slide-enter-active,
.fade-slide-leave-active {
  transition: all 0.3s ease;
}

.fade-slide-enter-from {
  opacity: 0;
  transform: translateY(-8px);
}

.fade-slide-leave-to {
  opacity: 0;
  transform: translateY(-8px);
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

.btn-ir-sorteo {
  position: sticky;
  top: 1rem;
  right: 1rem;

  margin-left: auto;
  /* para empujar a la derecha */
  display: block;

  z-index: 50;
  background-color: #16a34a;
  color: white;
  font-weight: 700;
  padding: 0.9rem 2rem;
  border-radius: 0.7rem;
  border: none;
  box-shadow: 0 6px 18px rgba(0, 0, 0, 0.25);
  transition: all 0.2s ease-in-out;
}

.btn-ir-sorteo:hover {
  background-color: #138a3e;
  transform: translateY(-2px);
}
</style>
