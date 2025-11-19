<template>
  <Dialog
    :visible="visible"
    modal
    @update:visible="$emit('cerrar')"
    header="Registro de Asistencia"
    class="modal-asistencia"
  >
    <div class="contenido-modal">

      <!-- 🔹 Totales -->
      <div class="totales-box">
        <p><strong>Total Asistencias:</strong> {{ totales.asistencias }}</p>
        <p><strong>Concursando:</strong> {{ totales.concursando }}</p>
      </div>

      <!-- 🔹 Input de cédula -->
      <div class="input-busqueda mt-3">
        <label class="form-label fw-bold">Buscar cédula</label>

        <div class="d-flex gap-2">
          <input
            v-model="cedula"
            class="form-control"
            placeholder="Ingrese cédula"
            @keyup.enter="buscarCedula"
          />

          <Button
            label="Buscar"
            icon="pi pi-search"
            class="p-button-info"
            @click="buscarCedula"
          />
        </div>
      </div>

      <!-- 🔹 Datos del participante -->
      <div v-if="datos" class="datos-box mt-4">
        <p><strong>Cédula:</strong> {{ datos.cedula }}</p>
        <p><strong>Nombre:</strong> {{ datos.nombres }} {{ datos.apellidos }}</p>
      </div>

      <!-- 🔹 Botón Registrar -->
      <div v-if="datos" class="text-center mt-4">
        <Button
          label="Registrar Asistencia"
          icon="pi pi-check"
          class="p-button-success p-button-lg"
          @click="registrar"
        />
      </div>

    </div>
  </Dialog>
</template>

<script>
import { ref } from "vue";
import api from "@/utilities/api";
import { useToast } from "primevue/usetoast";

export default {
  name: "ModalAsistencia",
  props: {
    visible: Boolean,
    sorteoId: Number,
  },

  setup(props, { emit }) {
    const toast = useToast();

    const cedula = ref("");
    const datos = ref(null);

    const totales = ref({
      asistencias: 0,
      concursando: 0,
    });

    // 🔹 Cargar totales al abrir modal
    const cargarTotales = async () => {
      try {
        const r = await api.get(`/sorteos/${props.sorteoId}/asistencia`);
        totales.value.asistencias = r.data.asistenciasTotal;
        totales.value.concursando = r.data.concursando;
      } catch (err) {
        console.error(err);
      }
    };

    // 🔹 Buscar cédula en RRHH
    const buscarCedula = async () => {
      if (!cedula.value.trim()) {
        toast.add({
          severity: "warn",
          summary: "Atención",
          detail: "Ingrese una cédula válida.",
          life: 3000,
        });
        return;
      }

      try {
        const r = await api.get(`/sorteos/${cedula.value}/asistencia/cedulas`);
        datos.value = r.data;

        toast.add({
          severity: "success",
          summary: "Datos encontrados",
          detail: `${datos.value.nombres} ${datos.value.apellidos}`,
          life: 2500,
        });

      } catch (e) {
        datos.value = null;
        toast.add({
          severity: "error",
          summary: "No encontrado",
          detail: "La cédula no existe en RRHH.",
          life: 3000,
        });
      }
    };

    // 🔹 Registrar asistencia (POST)
    const registrar = async () => {
      try {
        await api.post(`/sorteos/${props.sorteoId}/asistencia`, {
          cedula: datos.value.cedula,
        });

        toast.add({
          severity: "success",
          summary: "Asistencia registrada",
          detail: "Participante registrado correctamente.",
          life: 3000,
        });

        emit("registrado"); // para refrescar totales arriba en AdminView
        datos.value = null;
        cedula.value = "";
        cargarTotales();

      } catch (err) {
        toast.add({
          severity: "error",
          summary: "Error",
          detail: "No se pudo registrar la asistencia.",
          life: 3000,
        });
      }
    };

    return {
      cedula,
      datos,
      totales,
      buscarCedula,
      registrar,
      cargarTotales,
    };
  },

  watch: {
    // cargar totales cuando se abre
    visible(val) {
      if (val) this.cargarTotales();
    },
  },
};
</script>

<style scoped>
.modal-asistencia {
  width: 520px;
}

.totales-box {
  background: rgba(255,255,255,0.1);
  padding: 1rem;
  border-radius: .6rem;
  text-align: center;
  font-size: 1.1rem;
}

.datos-box {
  background: rgba(255,255,255,0.08);
  padding: 1rem;
  border-radius: .6rem;
  border: 1px solid rgba(255,255,255,0.2);
}
</style>
