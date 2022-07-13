/* Styles */
import '@/scss/main.scss'

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


import { getToken } from '@/utils/auth'

import VueSignalR from '@latelier/vue-signalr'
/* Fetch sample data */
//store.dispatch('fetch', 'clients')

/* Default title tag */
const defaultDocumentTitle = 'Automation tool'



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

Vue.mixin(mixin)

Vue.config.productionTip = false
Vue.config.devtools = process.env.NODE_ENV === 'development'
Vue.use(VueSignalR, `${process.env.VUE_APP_BASE_API}/hubClient`)

Vue.use(Buefy)
new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
