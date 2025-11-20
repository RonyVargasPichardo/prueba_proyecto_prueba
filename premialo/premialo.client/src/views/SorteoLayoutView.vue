<template>
    <div class="sorteo-layout container-fluid py-2 animate__animated animate__fadeIn">
        <!-- 🔹 HEADER (ya tiene su propio sticky y estilos internos) -->
        <Header :perfilUsuario="perfilUsuario" @cerrar-sesion="cerrarSesion" />

        <!-- 🔹 CONTENEDOR PRINCIPAL (Sidebar + Contenido dinámico) -->
        <div class="layout-body">
            <!-- Sidebar -->
            <SidebarMenu @navigate="cambiarVista" :active="vistaActual" />

            <!-- 🔹 Área de contenido (por ahora activa para probar proporciones) -->
            <main class="contenido-principal">
                <transition name="fade-slide" mode="out-in">
                    <router-view :sorteoId="$route.params.id" />
                </transition>
            </main>
        </div>
    </div>
</template>

<script>
import Header from "@/components/Header.vue";
import SidebarMenu from "@/components/SidebarMenu.vue";
import ConfiguracionView from "@/views/ConfiguracionView.vue";
import AsistenciaView from "@/views/AsistenciaView.vue";
import ParticipantesView from "@/views/ParticipantesView.vue";
import PremiosView from "@/views/PremiosView.vue";
import ResultadosView from "@/views/ResultadosView.vue";

export default {
    name: "SorteoLayoutView",
    components: {
        Header,
        SidebarMenu,
        ConfiguracionView,
        AsistenciaView,
        ParticipantesView,
        PremiosView,
        ResultadosView,
    },
    data() {
        return {
            vistaActual: "ConfiguracionView",
            perfilUsuario: localStorage.getItem("perfil") || "Administrador",
        };
    },
    computed: {
        vistaActualComponent() {
            return this.vistaActual;
        },
    },
    methods: {
        cerrarSesion() {
            localStorage.removeItem("perfil");
            this.$router.push("/login");
        },
    },
};
</script>

<style scoped>
/* ============================
   ESTRUCTURA GENERAL
============================ */
.sorteo-layout {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
    color: #fff;
}

/* ============================
   CONTENEDOR PRINCIPAL
============================ */
.layout-body {
    display: flex;
    flex: 1;
    margin-top: 1rem;
    gap: 1rem;
}

/* ============================
   SIDEBAR
============================ */
.sidebar-menu {
    flex: 0 0 260px;
    background: rgba(255, 255, 255, 0.08);
    backdrop-filter: blur(10px);
    border-right: 1px solid rgba(255, 255, 255, 0.15);
    border-radius: 0.8rem;
    padding: 1rem 0.5rem;

    height: 100vh;
}

/* ============================
   CONTENIDO PRINCIPAL
============================ */
/* ============================
   CONTENIDO PRINCIPAL (separado del sidebar)
============================ */
.contenido-principal {
    flex: 1;
    padding: 2rem;
    background: rgba(255, 255, 255, 0.06);
    border-radius: 1rem;
    border: 1px solid rgba(255, 255, 255, 0.1);
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.25);
    backdrop-filter: blur(8px);
    overflow-y: auto;

    /* ✅ Margen para despegarlo del sidebar */
    /* margin-left: 15rem; */
}

.fade-slide-enter-active,
.fade-slide-leave-active {
  transition: all 0.35s ease;
}
.fade-slide-enter-from {
  opacity: 0;
  transform: translateY(15px);
}
.fade-slide-leave-to {
  opacity: 0;
  transform: translateY(-15px);
}
</style>
