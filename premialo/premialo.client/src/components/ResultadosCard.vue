<template>
  <div class="resultados-card animate__animated animate__fadeInUp">

    <div class="titulo-card-contenedor">
      <h5 class="titulo-card">
        <i class="bi bi-stars me-2"></i>
        Resultados del Sorteo
      </h5>

      <button class="btn-descargar" @click="$emit('descargar-xls')">
        <i class="bi bi-file-earmark-excel me-1"></i>
        Descargar XLS
      </button>
    </div>

    <div v-if="!resultados?.length" class="alerta-vacia">
      <i class="bi bi-info-circle me-1"></i>
      Aún no se han registrado resultados del sorteo.
    </div>

    <div v-else class="tabla-contenedor">
      <table class="tabla-resultados">
        <thead>
          <tr>
            <th class="text-center">#</th>
            <th>Premio</th>
            <th>Ganador</th>
            <th>Fecha</th>
          </tr>
        </thead>

        <tbody>
          <tr v-for="(resultado, index) in resultados" :key="resultado.idresultado">
            <td class="text-center">{{ index + 1 }}</td>
            <td>{{ resultado.premio.nombre }}</td>
            <td>
              {{ resultado.participante.nombres }} {{ resultado.participante.apellidos }}
            </td>
            <td>
              {{ formatearFecha(resultado.fecha || "-") }}
            </td>
          </tr>
        </tbody>



      </table>
    </div>
    <div class="text-center total-ganadores">
      Total de ganadores: {{ totalGanadores }}
    </div>

  </div>
</template>


<script>
export default {
  name: "ResultadosCard",
  props: {
    resultados: { type: Array, default: () => [] },
    totalParticipantes: { type: Number, default: 0 },
    totalConcursando: { type: Number, default: 0 },
  },
  computed: {
    totalGanadores() {
      return this.resultados.length;
    }
  },
  methods: {
    formatearFecha(fecha) {
      if (!fecha) return "";
      const f = new Date(fecha);

      return f.toLocaleString("es-DO", {
        year: "numeric",
        month: "long",
        day: "numeric",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit"
      });
    }
  }
};
</script>

<style scoped>
.resultados-card {
  background: rgba(255, 255, 255, 0.06);
  /* igual que los módulos */
  backdrop-filter: blur(10px);
  border-radius: 1rem;
  padding: 1.5rem;
  border: 1px solid rgba(255, 255, 255, 0.10);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.25);

  margin: 0 auto;


  display: flex;
  flex-direction: column;
}

/* Contenedor del título + botón */
.titulo-card-contenedor {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

/* Título */
.titulo-card {
  font-weight: 700;
  color: #5cc4ff;
  font-size: 1.6rem;
  margin: 0;
}

/* Botón XLS */
.btn-descargar {
  background: rgba(34, 197, 94, 0.15);
  /* verde suave */
  color: #22c55e;
  border: 1px solid rgba(34, 197, 94, 0.4);
  padding: 0.45rem 0.9rem;
  border-radius: 0.5rem;
  font-weight: 600;
  font-size: 0.9rem;
  transition: all 0.25s ease;
  backdrop-filter: blur(4px);
}

.total-ganadores {
  font-weight: 700;
  color: #dbeafe;
  padding: 1rem 0;
  background: rgba(255, 255, 255, 0.07);
  /* border-top: 2px solid rgba(255, 255, 255, 0.15); */
  border: 1px solid rgba(255, 255, 255, 0.15);
  border-bottom-right-radius: 5px;
  border-bottom-left-radius: 5px;
}

.btn-descargar:hover {
  background: rgba(34, 197, 94, 0.28);
  border-color: rgba(34, 197, 94, 0.6);
  color: #16a34a;
  transform: translateY(-2px);
}

/* Mensaje vacío */
.alerta-vacia {
  background: rgba(255, 255, 255, 0.15);
  color: #fefefe;
  padding: 1rem;
  border-radius: .6rem;
  text-align: center;
}

/* Contenedor scroll */
.tabla-contenedor {
  overflow-y: auto;
  max-height: 380px;
  border-top-right-radius: 0.6rem;
  border-top-left-radius: 0.6rem;
  border: 1px solid rgba(255, 255, 255, 0.15);
}

/* TABLA */
.tabla-resultados {
  width: 100%;
  border-collapse: collapse;
  color: #f8fafc;
  font-size: 0.95rem;
}

.tabla-resultados thead {
  position: sticky;

  top: 0;
  background: rgb(30, 59, 138);
  /* azul más intenso */
  backdrop-filter: blur(6px);
  color: #bae6fd;
}

.tabla-resultados th {
  padding: 0.8rem;
  font-weight: 600;
  border-bottom: 2px solid rgba(255, 255, 255, 0.15);
  text-align: start;
}

.tabla-resultados td {
  padding: 0.9rem 0.8rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.06);
}

.tabla-resultados tr:hover {
  background: rgba(255, 255, 255, 0.08);
  transition: 0.25s ease;
}
</style>
