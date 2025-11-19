<template>
  <div class="admin-card animate__animated animate__fadeInUp">
    <h5 class="card-title">
      <i class="bi bi-person-dash-fill me-2 text-info"></i> Gestión de Excluidos
    </h5>
    <p class="small text-light opacity-75 mb-3">
      Cargar base de datos de empleados excluidos.
    </p>

    <!--Antes de cargar -->
    <div v-if="!archivoCargado">
      <button class="btn btn-danger w-100 fw-bold" @click="cargarArchivo">
        <i class="bi bi-upload me-2"></i> Cargar Base de Excluidos (.xlsx / .json)
      </button>
    </div>

    <!--Después de cargar -->
    <div v-else class="archivo-info animate__animated animate__fadeIn">
      <div class="archivo-card d-flex justify-content-between align-items-center px-3 py-2 mb-3">
        <div class="d-flex flex-column text-start">
          <small class="text-white-50">Archivo cargado:</small>
          <span class="fw-semibold text-white">{{ archivoNombre }}</span>
        </div>
        <button class="btn btn-sm btn-outline-danger" @click="eliminarArchivo">
          <i class="pi pi-times"></i>
        </button>
      </div>

      <button class="btn btn-outline-light w-100 fw-semibold mb-2" @click="$emit('ver-excluidos')">
        <i class="bi bi-eye me-1"></i> Ver Excluidos
      </button>
    </div>

    <!-- Totales -->
    <div class="text-white-50 small mt-3">
      <p class="mb-1">
        Total Excluidos: <strong>{{ totalExcluidos }}</strong>
      </p>
    </div>
  </div>
</template>

<script>
import * as XLSX from "xlsx";

export default {
  name: "ExcluidosCard",
  props: {
    totalExcluidos: { type: Number, default: 0 },
  },
  data() {
    return {
      archivoCargado: false,
      archivoNombre: "",
    };
  },
  methods: {
    async cargarArchivo() {
      const input = document.createElement("input");
      input.type = "file";
      input.accept = ".xlsx,.xls,.json";
      input.addEventListener("change", async (event) => {
        const file = event.target.files[0];
        if (!file) return;

        this.archivoNombre = file.name;
        this.archivoCargado = true;

        // Leer Excel
        const data = await file.arrayBuffer();
        const workbook = XLSX.read(data, { type: "array" });
        const sheetName = workbook.SheetNames[0];
        const sheet = workbook.Sheets[sheetName];
        const json = XLSX.utils.sheet_to_json(sheet, { defval: "" });

        // Emitir el contenido hacia AdminView
        this.$emit("archivo-excluidos", json);
      });
      input.click();
    },
    eliminarArchivo() {
      this.archivoCargado = false;
      this.archivoNombre = "";
      this.$emit("eliminar-excluidos");
    },
  },
};
</script>

<style scoped>
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

.card-title {
  font-weight: 700;
  color: #ff7575;
}

.archivo-card {
  background: rgba(255, 255, 255, 0.15);
  border-radius: 0.6rem;
  color: #fff;
  border: 1px solid rgba(255, 255, 255, 0.25);
}

.archivo-card button {
  color: #ff5b5b;
  border-color: rgba(255, 91, 91, 0.3);
  transition: all 0.2s ease;
}

.archivo-card button:hover {
  background-color: rgba(255, 91, 91, 0.15);
}
</style>
