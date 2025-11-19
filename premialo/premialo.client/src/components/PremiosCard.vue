<template>
  <div class="admin-card animate__animated animate__fadeInUp">

    <h5 class="card-title">
      <i class="bi bi-gift-fill me-2 text-info"></i> Gestión de Premios
    </h5>

    <p class="text-white-50 small mb-3">
      Ver la lista completa de premios.
    </p>

    <!-- Botón principal -->
    <button class="btn btn-primary w-100 fw-bold py-2" @click="$emit('ver-premios')">
      <i class="pi pi-gift me-1"></i>
      Ver Premios
    </button>


      <!-- SECCIÓN DE TOTALES -->
    <div class="totales-container mt-4">
      <div class="total-item">
        <label> Total de premios:</label>
        <span class="valor">{{ totalPremios }}</span>
      </div>
          <div class="total-item">
        <label> Premios Sorteados:</label>
        <span class="valor">{{ premiosSorteados }}</span>
      </div>
      <div class="total-item">
        <label> Premios Pendientes:</label>
        <span class="valor">{{ premiosPendientes }}</span>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "PremiosCard",
  props: {
    totalPremios: { type: Number, default: 0 },
    premiosPendientes: { type: Number, default: 0 },
    premiosSorteados: { type: Number, default: 0 },
  }
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
  height: 100%;
}

.admin-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 4px 15px rgba(255, 255, 255, 0.1);
}

.card-title {
  font-weight: 700;
  color: #5cc4ff;
}


/* Contenedor de totales */
.totales-container {
  background: rgba(255, 255, 255, 0.08);
  border-radius: 0.7rem;
  padding: 1rem 1.2rem;
  border: 1px solid rgba(255, 255, 255, 0.12);
}

.total-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.4rem;
  font-size: 1rem;
}

.total-item:last-child {
  margin-bottom: 0;
}

.total-item label {
  color: #dbeafe;
  font-weight: 600;
}

.valor {
  color: #fff;
  font-weight: 700;
}
</style>
