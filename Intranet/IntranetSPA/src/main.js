/* Styles */
import '@/scss/main.scss'
import '@/css/customize.css'
import 'vue-multiselect/dist/vue-multiselect.min.css'
/* Core */
import Vue from 'vue'
import Buefy from 'buefy'

/* Router & Store */
import router from './router'
import store from './store'

/* Vue. Main component */
import App from './App.vue'

/* add mixin */
import mixin from './utils/mixin'

/* add filter */
import './utils/filter';

/* add validate */
import VeeValidate from 'vee-validate';


import { getToken } from '@/utils/auth'

import VueSignalR from '@latelier/vue-signalr'
/* Fetch sample data */
//store.dispatch('fetch', 'clients')

/* Default title tag */
const defaultDocumentTitle = 'Intranet'

import VueMobileDetection from 'vue-mobile-detection'

router.beforeEach((to, from, next) => {
  let token=getToken()  
  //console.log(to.name);
  //console.log(token);
  if(!token && to.name!=='login') 
    next({ name: 'login' })
  next()
})

/* Collapse mobile aside menu on route change & set document title from route meta */
router.afterEach(to => {
  store.commit('asideMobileStateToggle', false)

  if (to.meta && to.meta.title) {
    document.title = `${to.meta.title} — ${defaultDocumentTitle}`
  } else {
    document.title = defaultDocumentTitle
  }
})


Vue.config.productionTip = false
Vue.config.devtools = process.env.NODE_ENV === 'development'
Vue.use(VueSignalR, `${process.env.VUE_APP_BASE_API}/hubClient`)
Vue.use(Buefy)
Vue.use(VueMobileDetection)
Vue.use(VeeValidate)
Vue.mixin(mixin)

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
