import { createApp } from 'vue'
import App from './App.vue'
import router from './router'


//PrimeVue Ecosystem
import PrimeVue from 'primevue/config'
import ToastService from 'primevue/toastservice'
import Toast from 'primevue/toast'
import InputGroup from 'primevue/inputgroup'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import Tooltip from 'primevue/tooltip'

// Tema moderno (PrimeVue 4+)
import Aura from '@primeuix/themes/aura'

//Estilos complementarios
import 'primeicons/primeicons.css'
import 'primeflex/primeflex.css'

//Estilos globales personalizados
import './style.css'

// Bootstrap (siempre primero)
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'


// ✨ Librerías de animación
import 'animate.css'
import 'aos/dist/aos.css'
import AOS from 'aos'
AOS.init()

// Inicialización de la app
const app = createApp(App)

// 🔹 Configuración de PrimeVue
app.use(PrimeVue, {
  ripple: true,
  locale: {
    apply: 'Aplicar',
    clear: 'Limpiar'
  },
  theme: {
    preset: Aura,
    options: {
      darkModeSelector: false
    }
  }
})

//Registrar componentes globales que usarás frecuentemente
app.component('InputGroup', InputGroup)
app.component('InputText', InputText)
app.component('Button', Button)
app.component('Dialog', Dialog)
app.component('Toast', Toast)

//Directiva y servicio global
app.directive('tooltip', Tooltip)
app.use(ToastService)

//Usa tu router
app.use(router)

//Monta la aplicación
app.mount('#app')
