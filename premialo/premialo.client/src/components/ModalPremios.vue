<template>
  <transition name="fade-overlay">
    <div v-if="visible" class="overlay-opaco"></div>
  </transition>

  <transition name="fade-modal">
    <div v-if="visible" class="modal-container">
      <div class="modal-dialog modal-xl modal-dialog-centered">
        <div class="modal-content moderno-modal shadow-xl border-0">

          <!-- ================= HEADER ================= -->
          <div class="modal-header header-gradiente">
            <div class="d-flex align-items-center gap-2 w-100 justify-content-between">
              <h5 class="modal-title fw-bold text-white mb-0">
                <i class="pi pi-gift me-2"></i> Lista de Premios
              </h5>

              <div class="filtro-mini d-flex align-items-center bg-white px-2 rounded-3">
                <i class="pi pi-search text-primary small"></i>
                <input type="text" v-model="filtroNombre" class="filtro-mini-input" placeholder="Premio..." />
              </div>
            </div>

            <button class="btn-close btn-close-white ms-2" @click="cerrar"></button>
          </div>

          <!-- ================= BODY ================= -->
          <div class="modal-body px-4 py-3">

            <!-- Agregar de uno -->
            <div class="row mb-4">
              <div class="col-md-9">
                <input v-model="premioNuevo" type="text" placeholder="Nombre del premio" class="form-control" />
              </div>
              <div class="col-md-3 d-flex justify-content-end">
                <button class="btn btn-gradiente w-100 fw-semibold bg-blue" @click="abrirModalCrear">
                  <i class="pi pi-plus me-2"></i> Agregar premio
                </button>
              </div>
            </div>

            <!-- Tabla -->
            <div class="table-responsive table-wrapper">
              <table class="table tabla-compacta align-middle">
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Nombre</th>
                    <th>Descripción</th>
                    <th>Orden</th>
                    <th>Estatus</th>
                    <th class="text-center">Acción</th>
                  </tr>
                </thead>

                <tbody>

                  <!-- Sin premios -->
                  <tr v-if="premios.length === 0">
                    <td colspan="5" class="text-center text-muted py-3">
                      <i class="pi pi-gift me-2"></i>
                      Aún no se han cargado premios para este sorteo.
                    </td>
                  </tr>

                  <!-- Filtro sin resultados -->
                  <tr v-else-if="premiosFiltrados.length === 0">
                    <td colspan="5" class="text-center text-muted py-3">
                      <i class="pi pi-info-circle me-2"></i>
                      No se encontraron premios con ese nombre.
                    </td>
                  </tr>

                  <!-- Premios -->
                  <tr v-for="(p, index) in premiosFiltrados" :key="p.id">
                    <td>{{ index + 1 }}</td>

                    <!-- NOMBRE -->
                    <td class="fw-semibold">
                      {{ p.nombre }}
                    </td>

                    <!-- DESCRIPCIÓN -->
                    <td class="text-muted" style="max-width: 260px; white-space: normal;">
                      {{ p.descripcion || '—' }}
                    </td>


                    <!-- ORDEN -->
                    <td>
                      <span class="">
                        {{ p.orden }}
                      </span>
                    </td>

                    <!-- ESTATUS -->
                    <td>
                      <span class="badge" :class="{
                        'bg-info': p.estatusNombre === 'Sorteado',
                        'bg-success': p.estatusNombre === 'Entregado',
                        'bg-secondary': !p.estatusNombre || p.estatusNombre === 'Pendiente'
                      }">
                        {{ p.estatusNombre || 'Pendiente' }}
                      </span>
                    </td>

                    <!-- ACCIONES -->
                    <td class="text-center">

                      <!-- EDITAR -->
                      <button class="btn btn-sm me-2" @click="abrirEditor(p)">
                        <i class="pi pi-pencil"></i>
                      </button>

                      <!-- BORRAR -->
                      <button class="btn btn-sm" @click="confirmarEliminarUnidad(p.id)">
                        <i class="pi pi-trash"></i>
                      </button>

                    </td>
                  </tr>

                </tbody>

              </table>
            </div>

          </div>

          <!-- ================= FOOTER ================= -->
          <div class="modal-footer justify-content-center border-0 pb-4 gap-3">

            <button class="btn btn-gradiente-azul px-4 fw-semibold"
              @click="premios.length ? abrirConfirmacion() : triggerFile()">
              <i class="pi pi-upload me-2"></i>
              {{ premios.length ? 'Cambiar Base de Datos (.xlsx)' : 'Cargar Base de Datos (.xlsx)' }}
            </button>

            <input ref="inputArchivo" type="file" accept=".xlsx,.xls" class="d-none" @change="procesarArchivo" />

            <button class="btn btn-gradiente px-4 fw-semibold" @click="cerrar">
              <i class="pi pi-times me-2"></i> Cerrar
            </button>
          </div>

          <!-- ================= ELIMINAR TODO ================= -->
          <div v-if="modalConfirmar" class="modal-confirm">
            <div class="modal-confirm-box">
              <h4 class="fw-bold text-primary mb-2">¿Reemplazar premios?</h4>
              <p class="text-muted small">Esto eliminará todos los premios actuales del sorteo.</p>

              <div class="d-flex gap-3 justify-content-center mt-3">
                <button class="btn btn-secondary" @click="modalConfirmar = false">No</button>
                <button class="btn btn-danger" @click="confirmarEliminacion">Sí, eliminar todo</button>
              </div>
            </div>
          </div>

          <!-- ================= ELIMINAR UNIDAD ================= -->
          <div v-if="modalEliminarUnidad" class="modal-confirm">
            <div class="modal-confirm-box">
              <h4 class="fw-bold text-danger mb-2">¿Eliminar este premio?</h4>

              <div class="d-flex gap-3 justify-content-center mt-3">
                <button class="btn btn-secondary" @click="modalEliminarUnidad = false">Cancelar</button>
                <button class="btn btn-danger" @click="eliminarPremioUnidad">Eliminar</button>
              </div>
            </div>
          </div>

          <!-- ================= CARGA XLSX OK ================= -->
          <div v-if="modalCargaExitosa" class="modal-confirm">
            <div class="modal-confirm-box">
              <h4 class="fw-bold text-success mb-2">¡Carga exitosa!</h4>
              <p class="text-muted small">La base de premios fue cargada correctamente.</p>
              <button class="btn btn-primary mt-3" @click="modalCargaExitosa = false">Entendido</button>
            </div>
          </div>

          <!-- =============== MODAL CREAR PREMIO =============== -->
          <div v-if="modalCrear" class="modal-confirm">
            <div class="modal-confirm-box" style="max-width: 500px; width: 90%;">

              <h4 class="fw-bold text-primary mb-3">
                Nuevo premio
              </h4>

              <input v-model="nuevoNombre" type="text" class="form-control mb-3" placeholder="Nombre del premio"
                style="padding: 0.8rem; font-size: 1rem;" />

              <textarea v-model="nuevoDescripcion" class="form-control mb-3"
                placeholder="Descripción del premio (opcional)"
                style="padding: 0.8rem; font-size: 1rem; min-height: 90px;"></textarea>

              <div class="d-flex gap-3 justify-content-center mt-2">
                <button class="btn btn-secondary" @click="modalCrear = false">
                  Cancelar
                </button>

                <button class="btn btn-primary" @click="crearPremio">
                  Guardar premio
                </button>
              </div>

            </div>
          </div>


          <!-- =============== MODAL EDITAR PREMIO (MISMO ARCHIVO) =============== -->
          <div v-if="modalEditar" class="modal-confirm">
            <div class="modal-confirm-box" style="max-width: 500px; width: 90%;">

              <h4 class="fw-bold text-primary mb-3">
                Editar premio
              </h4>

              <input v-model="premioEditandoTexto" type="text" class="form-control mb-3"
                placeholder="Nuevo nombre del premio" style="padding: 0.8rem; font-size: 1rem;" />

              <textarea v-model="premioEditandoDescripcion" class="form-control mb-3"
                placeholder="Descripción del premio (opcional)"
                style="padding: 0.8rem; font-size: 1rem; min-height: 90px;"></textarea>

              <div class="d-flex gap-3 justify-content-center mt-2">
                <button class="btn btn-secondary" @click="cerrarEditor">
                  Cancelar
                </button>

                <button class="btn btn-primary" @click="guardarEdicion">
                  Guardar cambios
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
import * as XLSX from "xlsx";
import { useToast } from "primevue/usetoast";

export default {
  name: "ModalPremios",

  props: {
    visible: Boolean,
    sorteoId: Number
  },

  setup() {
    const toast = useToast();
    return { toast };
  },

  data() {
    return {
      premios: [],
      filtroNombre: "",
      premioNuevo: "",
      loading: false,

      // crear
      modalCrear: false,
      nuevoNombre: "",
      nuevoDescripcion: "",

      // EDITOR INTERNO
      modalEditar: false,
      premioEditandoId: null,
      premioEditandoTexto: "",
      premioEditandoDescripcion: "",
      premioEditandoOrden: 0,

      modalConfirmar: false,
      modalEliminarUnidad: false,
      idUnidadEliminar: null,
      modalCargaExitosa: false
    };
  },

  computed: {
    premiosFiltrados() {
      if (!this.filtroNombre.trim()) return this.premios;
      const t = this.filtroNombre.toLowerCase();
      return this.premios.filter(p => p.nombre.toLowerCase().includes(t));
    }
  },

  methods: {
    // GENERAL
    cerrar() {
      this.$emit("cerrar");
    },

    triggerFile() {
      this.$refs.inputArchivo.click();
    },

    async cargarPremios() {
      try {
        const { data } = await api.get(`/sorteos/${this.sorteoId}/premios`);
        this.premios = data;
      } catch (e) {
        console.error("Error cargando premios:", e);
      }
    },
    abrirModalCrear() {
      this.nuevoNombre = "";
      this.nuevoDescripcion = "";
      this.modalCrear = true;
    },

    // ================== AGREGAR UNO ==================
    async crearPremio() {
      if (!this.nuevoNombre.trim()) return;

      try {
        await api.post(`/sorteos/${this.sorteoId}/premios`, {
          nombre: this.nuevoNombre,
          descripcion: this.nuevoDescripcion,
          orden: this.premios.length + 1
        });

        this.modalCrear = false;
        await this.cargarPremios();

        this.toast.add({
          severity: "success",
          summary: "Premio agregado",
          life: 3000
        });
      } catch (e) {
        console.error(e);
      }
    },

    // ================== EDITAR ==================
    abrirEditor(p) {
      this.premioEditandoId = p.id;
      this.premioEditandoTexto = p.nombre;
      this.premioEditandoDescripcion = p.descripcion || "";
      this.premioEditandoOrden = p.orden || 0;
      this.modalEditar = true;
    },

    cerrarEditor() {
      this.modalEditar = false;
      this.premioEditandoTexto = "";
      this.premioEditandoDescripcion = "";
      this.premioEditandoId = null;
    },

    async guardarEdicion() {
      if (!this.premioEditandoTexto.trim()) return;

      try {
        await api.put(`/sorteos/${this.sorteoId}/premios/${this.premioEditandoId}`, {
          nombre: this.premioEditandoTexto,
          descripcion: this.premioEditandoDescripcion,
          orden: this.premioEditandoOrden
        });

        this.modalEditar = false;
        await this.cargarPremios();

        this.toast.add({
          severity: "success",
          summary: "Premio actualizado",
          life: 3000
        });

      } catch (e) {
        console.error(e);
      }
    },

    // ================== XLSX ==================
    mapearPremio(item) {
      return {
        nombre: item.Nombre || item.nombre || item.Premio || "",
        descripcion: item.Descripcion || item.descripcion || "",
        orden: item.Orden || item.orden || 0
      };
    },

    async procesarArchivo(e) {
      const file = e.target.files[0];
      if (!file) return;

      try {
        const buffer = await file.arrayBuffer();
        const workbook = XLSX.read(buffer);
        const hoja = workbook.Sheets[workbook.SheetNames[0]];
        const json = XLSX.utils.sheet_to_json(hoja, { defval: "" });

        let lista = json
          .map(this.mapearPremio)
          .filter(x => x.nombre.trim() !== "");

        var reqObj = { premios: lista };

        await api.post(`/sorteos/${this.sorteoId}/premios/json`, reqObj);

        await this.cargarPremios();
        this.modalCargaExitosa = true;

      } catch (err) {
        console.log(err);
        this.toast.add({
          severity: "error",
          summary: "Error al cargar XLSX",
          detail: "Archivo inválido o datos duplicados.",
          life: 4500
        });
      }
    },

    // ================== ELIMINAR TODO ==================
    abrirConfirmacion() {
      this.modalConfirmar = true;
    },

    async confirmarEliminacion() {
      try {
        await api.delete(`/sorteos/${this.sorteoId}/premios`);
        this.premios = [];
        this.modalConfirmar = false;
      } catch (e) { console.error(e); }
    },

    // ================== ELIMINAR 1 ==================
    confirmarEliminarUnidad(id) {
      this.idUnidadEliminar = id;
      this.modalEliminarUnidad = true;
    },

    async eliminarPremioUnidad() {
      try {
        await api.delete(`/sorteos/${this.sorteoId}/premios/${this.idUnidadEliminar}`);
        this.modalEliminarUnidad = false;
        await this.cargarPremios();
      } catch (e) { console.error(e); }
    }
  },

  watch: {
    visible(val) {
      document.body.style.overflow = val ? "hidden" : "auto";
      if (val) this.cargarPremios();
    }
  }
};
</script>


<style scoped>
/* ================================
   OVERLAY OSCURO
================================ */
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

/* ================================
   CONTENEDOR DEL MODAL
================================ */
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

/* ================================
   MODAL PRINCIPAL
================================ */
.moderno-modal {
  background: #f9fafc;
  color: #1e293b;
  border-radius: 1rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.45);
  animation: scaleIn 0.35s ease forwards;
  max-height: 90vh;
  min-height: 90vh;
  min-width: 60vw;
  overflow-y: auto;
}

/* ================================
   HEADER DEL MODAL
================================ */
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

/* ================================
   BUSCADOR MINI
================================ */
.filtro-mini {
  background: white;
  display: flex;
  align-items: center;
  border-radius: 8px;
  padding: 0 10px;
  height: 36px;
  border: 1px solid #d1d5db;
  min-width: 180px;
}

.filtro-mini-input {
  border: none;
  outline: none;
  background: transparent;
  margin-left: 6px;
  font-size: 0.85rem;
  width: 100%;
}

.filtro-mini-input::placeholder {
  color: #9ca3af;
}

/* ================================
   TABLA
================================ */
.table-wrapper {
  max-height: 60vh;
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


/* ================================
   BOTONES
================================ */
.btn-gradiente {
  background: linear-gradient(90deg, #0c5399, #0078d4);
  color: white;
  border: none;
  border-radius: 6px;
  padding: 0.6rem 1.3rem;
  transition: 0.25s;
}

.btn-gradiente:hover {
  transform: scale(1);
  box-shadow: 0 0 10px rgba(0, 86, 214, 0.3);
}

.btn-gradiente-azul {
  background: linear-gradient(90deg, #007bff, #0056d6);
  color: white;
  border: none;
  border-radius: 6px;
  padding: 0.6rem 1.3rem;
  transition: 0.25s;
}

.btn-gradiente-azul:hover {
  transform: scale(1);
  box-shadow: 0 0 12px rgba(0, 86, 214, 0.45);
}

/* ================================
   MODALES DE CONFIRMACIÓN
================================ */
.modal-confirm {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.45);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 3000;
}

.modal-confirm-box {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  max-width: 450px;
  width: 90%;
  text-align: center;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.25);
  animation: scaleIn 0.25s ease;
}

/* ================================
   MODAL EDITOR (INTERNO)
================================ */
.modal-confirm-box input {
  font-size: 1rem;
  padding: 0.8rem;
}

/* ================================
   ANIMACIONES
================================ */
@keyframes scaleIn {
  from {
    opacity: 0;
    transform: scale(0.92);
  }

  to {
    opacity: 1;
    transform: scale(1);
  }
}

</style
