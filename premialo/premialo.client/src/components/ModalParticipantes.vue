<template>
  <transition name="fade-overlay">
    <div v-if="visible" class="overlay-opaco"></div>
  </transition>

  <transition name="fade-modal">
    <div v-if="visible" class="modal-container">
      <div class="modal-dialog modal-xl modal-dialog-centered">
        <div class="modal-content moderno-modal shadow-xl border-0">
          <!-- HEADER -->
          <div class="modal-header header-gradiente">
            <div class="d-flex align-items-center gap-2 w-100 justify-content-between">
              <h5 class="modal-title fw-bold text-white mb-0">
                <i class="pi pi-users me-2"></i> Lista de Participantes
              </h5>

              <!-- 🔹 Input de filtro pequeño -->
              <div class="filtro-mini d-flex align-items-center bg-white px-2 rounded-3">
                <i class="pi pi-search text-primary small"></i>
                <input type="text" v-model="filtroCedula" class="filtro-mini-input" placeholder="Cédula..." />
              </div>
            </div>
            <button class="btn-close btn-close-white ms-2" @click="$emit('cerrar')"></button>
          </div>

          <!-- BODY -->
          <div class="modal-body px-4 py-3">
            <div class="table-responsive table-wrapper">
              <table class="table tabla-compacta align-middle">
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Nombre</th>
                    <th>Cédula</th>
                    <th>Departamento</th>
                    <th>Cargo</th>
                    <th class="text-center">Estatus</th>
                  </tr>
                </thead>

                <tbody>
                  <tr v-for="(p, index) in participantesFiltrados" :key="index"
                    :class="{ 'fila-activa': p.estatus === 'Concursando' }">
                    <td>{{ index + 1 }}</td>
                    <td class="fw-semibold">{{ p.nombres }} {{ p.apellidos }}</td>
                    <td>{{ p.documentoIdentidad }}</td>
                    <td>{{ p.unidadOrganizativa }}</td>
                    <td>{{ p.cargo }}</td>

                    <td class="text-center">
                      <span class="estado" :class="getClaseEstatus(p.estatus)">
                        {{ p.estatus }}
                      </span>
                    </td>
                  </tr>

                  <!--Caso 1: No hay NINGÚN participante cargado -->
                  <tr v-if="participantes.length === 0">
                    <td colspan="6" class="text-center text-muted py-3">
                      <i class="pi pi-users me-2"></i>
                      Aún no se han cargado participantes para este sorteo.
                    </td>
                  </tr>

                  <!--Caso 2: Sí hay participantes, pero el filtro no encuentra resultados -->
                  <tr v-else-if="participantesFiltrados.length === 0">
                    <td colspan="6" class="text-center text-muted py-3">
                      <i class="pi pi-info-circle me-2"></i>
                      No se encontraron participantes con esa cédula.
                    </td>
                  </tr>

                </tbody>
              </table>

            </div>
          </div>

          <!-- FOOTER -->
          <div class="modal-footer justify-content-center border-0 pb-4 gap-3">

            <!-- Cargar Base de Datos -->
            <button class="btn btn-gradiente-azul px-4 fw-semibold"
              @click="hayParticipantes ? abrirConfirmacion() : triggerFile()">
              <i class="pi pi-upload me-2"></i>
              {{ hayParticipantes ? 'Cambiar Base de Datos (.xlsx)' : 'Cargar Base de Datos (.xlsx)' }}
            </button>


            <input ref="inputArchivo" type="file" accept=".xlsx,.xls" class="d-none" @change="procesarArchivo" />

            <!-- Botón cerrar -->
            <button class="btn btn-gradiente px-4 fw-semibold" @click="cerrar()">
              <i class="pi pi-times me-2"></i> Cerrar
            </button>

          </div>

          <!-- MODAL DE CONFIRMACIÓN -->
          <div v-if="modalConfirmar" class="modal-confirm">
            <div class="modal-confirm-box">
              <h4 class="fw-bold text-primary mb-2">¿Reemplazar participantes?</h4>
              <p class="text-muted small">
                Esto eliminará todos los participantes actuales del sorteo.
              </p>

              <div class="d-flex gap-3 justify-content-center mt-3">
                <button class="btn btn-secondary" @click="modalConfirmar = false">No</button>
                <button class="btn btn-danger" @click="confirmarEliminacion">Sí, reemplazar</button>
              </div>
            </div>
          </div>

          <!-- MODAL DE RESULTADO Eliminacion -->
          <div v-if="modalResultado" class="modal-confirm">
            <div class="modal-confirm-box">
              <h4 class="fw-bold text-success mb-2">Participantes eliminados</h4>
              <p class="text-muted small">Ahora puedes subir una nueva base de datos.</p>

              <div class="d-flex gap-3 justify-content-center mt-3">
                <button class="btn btn-primary" @click="subirNuevaBase">Subir nueva Base (.xlsx)</button>
                <button class="btn btn-secondary" @click="cerrarResultado">Cerrar</button>
              </div>
            </div>
          </div>

          <!-- MODAL DE CARGA EXITOSA -->
          <div v-if="modalCargaExitosa" class="modal-confirm">
            <div class="modal-confirm-box">
              <h4 class="fw-bold text-success mb-2">¡Carga exitosa!</h4>
              <p class="text-muted small">
                La base de datos de participantes se cargó correctamente.
              </p>

              <div class="d-flex gap-3 justify-content-center mt-3">
                <button class="btn btn-primary" @click="modalCargaExitosa = false">
                  Entendido
                </button>
              </div>
            </div>
          </div>

        </div>
      </div>
    </div>
  </transition>
</template>

<script>
import api from "@/utilities/api";
import * as XLSX from 'xlsx';
import { useToast } from "primevue/usetoast";

export default {
  name: "ModalParticipantes",
  props: {
    visible: Boolean,
    sorteoId: { type: Number, required: true }
  },
  setup() {
    const toast = useToast();
    return { toast };
  },
  data() {
    return {
      filtroCedula: "",
      participantes: [],
      loading: false,

      modalCargaExitosa: false,
      modalConfirmar: false,
      modalResultado: false
    };
  },

  methods: {
    async cargarParticipantes() {
      this.loading = true;
      try {
        const { data } = await api.get(`/sorteos/${this.sorteoId}/participantes`);
        this.participantes = data;
      } catch (error) {
        console.error("Error al cargar participantes:", error);
      } finally {
        this.loading = false;
      }
    },
    getClaseEstatus(estatus) {
      switch (estatus) {
        case 'Registrado':
          return 'estado-registrado'
        case 'Concursando':
          return 'estado-concursando'
        case 'Ganador':
          return 'estado-ganador'
        default:
          return 'estado-registrado'
      }
    },
    cerrar() {
      this.$emit("cerrar");
    },

    triggerFile() {
      this.$refs.inputArchivo.click();
    },

    mapearItemExcel(item) {
      return {
        documentoIdentidad: item.documentoIdentidad || item.Cédula || item.Documento || "",
        nombres: item.nombres || item.Nombre || item.Nombres || "",
        apellidos: item.apellidos || item.Apellido || item.Apellidos || "",
        email: item.email || item.Email || "",
        telefono: item.telefono || item.Telefono || "",
        cargo: item.cargo || item.Cargo || "",
        unidadOrganizativa: item.unidadOrganizativa || item.Departamento || item.Unidad || ""
      };
    },
    async procesarArchivo(event) {
      const archivo = event.target.files[0];
      if (!archivo) return;

      try {
        const data = await archivo.arrayBuffer();
        const workbook = XLSX.read(data);

        const hoja = workbook.Sheets[workbook.SheetNames[0]];
        const json = XLSX.utils.sheet_to_json(hoja, { defval: "" });

        // Mapear
        let listaMapeada = json.map(this.mapearItemExcel);

        // Filtrar documentos inválidos
        listaMapeada = listaMapeada.filter(x =>
          x.documentoIdentidad &&
          x.documentoIdentidad.toString().trim().length >= 8
        );

        if (listaMapeada.length === 0) {
          this.toast.add({
            severity: "warn",
            summary: "Archivo inválido",
            detail: "El XLSX no contiene datos válidos.",
            life: 4500,
          });
          return;
        }

        // Enviar al backend
        await api.post(`/sorteos/${this.sorteoId}/participantes/bulk`, listaMapeada);

        // Recargar lista sin cerrar el modal
        await this.cargarParticipantes();

        // Mostrar toast de éxito
        this.toast.add({
          severity: "success",
          summary: "Participantes cargados",
          detail: "La lista se cargó correctamente.",
          life: 4000,
        });

        // Mostrar mini-modal opcional
        this.modalCargaExitosa = true;

      } catch (error) {
        console.error("Error procesando archivo:", error);

        this.toast.add({
          severity: "error",
          summary: "Error al procesar archivo",
          detail: "El XLSX contiene datos inválidos o registros duplicados.",
          life: 4500,
        });
      }
    },

    abrirConfirmacion() {
      this.modalConfirmar = true;
    },

    async confirmarEliminacion() {
      try {
        await api.delete(`/sorteos/${this.sorteoId}/participantes`);

        this.participantes = [];

        this.modalConfirmar = false;
        this.modalResultado = true;

      } catch (err) {
        console.error("Error eliminando participantes:", err);
        alert("Error eliminando participantes.");
      }
    },

    subirNuevaBase() {
      this.modalResultado = false;
      this.triggerFile(); // Abrir selector .xlsx
    },

    cerrarResultado() {
      this.modalResultado = false;
    },
  },

  computed: {
    participantesFiltrados() {
      if (!this.filtroCedula.trim()) return this.participantes;
      const texto = this.filtroCedula.toLowerCase();
      return this.participantes.filter(p =>
        p.documentoIdentidad?.toLowerCase().includes(texto)
      );
    },
    hayParticipantes() {
      return this.participantes && this.participantes.length > 0;
    }
  },

  watch: {
    visible(val) {
      document.body.style.overflow = val ? "hidden" : "auto";
      if (val) this.cargarParticipantes();
    }
  }
};
</script>


<style scoped>
/* ============================================================
   🔹 INPUT DE FILTRO (HEADER)
============================================================ */
.filtro-mini {
  position: relative;
  display: flex;
  align-items: center;
  background: rgba(255, 255, 255, 0.85);
  backdrop-filter: blur(6px);
  border: 1px solid rgba(0, 56, 112, 0.2);
  border-radius: 0.6rem;
  height: 34px;
  min-width: 170px;
  padding: 0 0.75rem;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
  transition: all 0.25s ease-in-out;
}

.filtro-mini i {
  font-size: 0.9rem;
  color: #003870;
  opacity: 0.75;
  transition: color 0.25s ease;
}

.filtro-mini-input {
  border: none;
  outline: none;
  background: transparent;
  font-size: 0.85rem;
  color: #1e293b;
  width: 100%;
  margin-left: 0.5rem;
  font-weight: 500;
}

.filtro-mini-input::placeholder {
  color: #94a3b8;
  font-weight: 400;
}

.filtro-mini:hover {
  border-color: rgba(0, 88, 176, 0.4);
  box-shadow: 0 0 4px rgba(0, 88, 176, 0.2);
}

.filtro-mini:focus-within {
  background: rgba(255, 255, 255, 0.95);
  border-color: #0058b0;
  box-shadow: 0 0 0 3px rgba(0, 88, 176, 0.25);
}

@media (max-width: 768px) {
  .filtro-mini {
    min-width: 130px;
    height: 32px;
  }

  .filtro-mini-input {
    font-size: 0.8rem;
  }
}

/* ============================================================
   🔹 OVERLAY Y CONTENEDOR PRINCIPAL
============================================================ */
.overlay-opaco {
  position: fixed;
  top: 5;
  left: 0;
  right: 0;
  bottom: 0;
  width: 100%;
  height: 100%;
  background: rgba(15, 23, 42, 0.65);
  z-index: 2000;
}

.modal-container {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2001;
  overflow: hidden;
  margin: 0;
  padding: 0;
}

/* ============================================================
   🔹 MODAL PRINCIPAL
============================================================ */
.moderno-modal {
  background: #f9fafc;
  color: #1e293b;
  border-radius: 1rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.45);
  animation: scaleIn 0.35s ease forwards;
  max-height: 90vh;
  min-height: 90vh;
  overflow-y: auto;
}

/* ============================================================
   🔹 HEADER DEL MODAL
============================================================ */
.header-gradiente {
  background: linear-gradient(90deg, #003870, #0058b0);
  color: white;
  border: none;
  padding: 1rem 1.5rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  box-shadow: 0 3px 6px rgba(0, 0, 0, 0.1);
  position: sticky;
  top: 0;
  z-index: 10;
}

/* ============================================================
   🔹 TABLA (CON ALTURA ESTABLE)
============================================================ */
.table-wrapper {
  max-height: 70vh;
  min-height: 300px;
  /* Mantiene altura constante */
  overflow-y: auto;
  background-color: white;
  border-radius: 0.6rem;
  box-shadow: inset 0 0 8px rgba(0, 0, 0, 0.05);
  transition: all 0.25s ease;
}

/* Evita que se encoja cuando no hay resultados */
.tabla-compacta tbody {
  min-height: 250px;
  display: table-row-group;
}

.tabla-compacta tbody tr:last-child td {
  border-bottom: none;
}

.tabla-compacta {
  width: 100%;
  border-collapse: collapse;
}

.tabla-compacta thead {
  background-color: #003870;
  color: #fff;
  position: sticky;
  top: 0;
  z-index: 5;
}

.tabla-compacta th {
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.03em;
  padding: 0.8rem;
  font-size: 0.85rem;
  border-bottom: 2px solid #e2e8f0;
}

.tabla-compacta td {
  padding: 0.75rem 0.8rem;
  border-bottom: 1px solid #e5e7eb;
}

.tabla-compacta tbody tr:hover {
  background-color: #f1f5fb;
  transition: background-color 0.2s ease;
}

/* ============================================================
   🔹 FILAS Y ESTADOS
============================================================ */
.fila-activa {
  background-color: rgba(22, 163, 74, 0.1);
}

.estado {
  display: inline-block;
  padding: 0.35em 0.7em;
  font-size: 0.8rem;
  border-radius: 0.5rem;
  font-weight: 600;
}


.estado-registrado {
  background-color: #9ca3af; /* gray-400 */
  color: white;
}


.estado-concursando {
  background-color: #3b82f6; /* blue-500 */
  color: white;
}

.estado-ganador {
  background-color: #22c55e; /* green-500 */
  color: white;
}
/* ============================================================
   🔹 BOTÓN GRADIENTE (FOOTER)
============================================================ */
.btn-gradiente {
  background: linear-gradient(90deg, #003870, #0058b0);
  color: white;
  border: none;
  border-radius: 0.6rem;
  padding: 0.6rem 1.3rem;
  transition: all 0.3s ease;
}

.btn-gradiente:hover {
  transform: scale(1.04);
  box-shadow: 0 0 10px rgba(0, 56, 112, 0.25);
}

/* ============================================================
   🔹 SCROLLBAR PERSONALIZADA
============================================================ */
.table-wrapper::-webkit-scrollbar {
  width: 8px;
}

.table-wrapper::-webkit-scrollbar-thumb {
  background: rgba(0, 56, 112, 0.4);
  border-radius: 10px;
}

.table-wrapper::-webkit-scrollbar-thumb:hover {
  background: rgba(0, 56, 112, 0.6);
}

/* ============================================================
   🔹 ANIMACIONES Y TRANSICIONES
============================================================ */
@keyframes scaleIn {
  from {
    opacity: 0;
    transform: scale(0.95);
  }

  to {
    opacity: 1;
    transform: scale(1);
  }
}

.fade-overlay-enter-active,
.fade-overlay-leave-active {
  transition: opacity 0.3s ease;
}

.fade-overlay-enter-from,
.fade-overlay-leave-to {
  opacity: 0;
}

.fade-modal-enter-active,
.fade-modal-leave-active {
  transition: opacity 0.3s ease, transform 0.35s ease;
}

.fade-modal-enter-from,
.fade-modal-leave-to {
  opacity: 0;
  transform: scale(0.95);
}

/* ============================================================
   🔹 DIMENSIONES DEL MODAL
============================================================ */
.modal-dialog.modal-xl {
  max-width: 1200px;
  width: 95%;
  margin: 0 !important;
}

.btn-gradiente-azul {
  background: linear-gradient(90deg, #007bff, #0056d6);
  color: white;
  border: none;
  border-radius: 0.6rem;
  padding: 0.6rem 1.3rem;
  transition: all 0.3s ease;
}

.btn-gradiente-azul:hover {
  transform: scale(1.04);
  box-shadow: 0 0 12px rgba(0, 86, 214, 0.4);
}

.modal-confirm {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 3000;
}

.modal-confirm-box {
  background: white;
  padding: 1.8rem;
  border-radius: 0.8rem;
  width: 90%;
  max-width: 420px;
  text-align: center;
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.25);
}


@media (max-width: 992px) {
  .modal-dialog.modal-xl {
    max-width: 95%;
    width: 95%;
  }

  .table-wrapper {
  max-height: 50vh;

}
}

</style>
