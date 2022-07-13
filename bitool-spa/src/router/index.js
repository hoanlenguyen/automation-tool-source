import Vue from 'vue'
import VueRouter from 'vue-router'
//import Home from '@/views/Home.vue'

Vue.use(VueRouter)

const routes = [
  // {
  //   // Document title tag
  //   // We combine it with defaultDocumentTitle set in `src/main.js` on router.afterEach hook
  //   meta: {
  //     title: 'Dashboard'
  //   },
  //   path: '/',
  //   name: 'home',
  //   component: Home
  // },
  {
    meta: {
      title: 'Import data'
    },
    path: '/import-data',
    name: 'import-data',    
    component: () => import('@/views/ImportData.vue')
  },
  {
    meta: {
      title: 'Import data history'
    },
    path: '/import-history',
    name: 'import-history',    
    component: () => import('@/views/ImportHistory.vue')
  },
  {
    meta: {
      title: 'Clean data history'
    },
    path: '/clean-history',
    name: 'clean-history',    
    component: () => import('@/views/CleanDataHistory.vue')
  },
  {
    meta: {
      title: 'Overall Database Report'
    },
    path: '/overall-report',
    name: 'overall-report',    
    component: () => import('@/views/OverallReport.vue')
  },
  {
    meta: {
      title: 'Export data'
    },
    path: '/export-data',
    name: 'export-data',    
    component: () => import('@/views/ExportData.vue')
  },
  {
    meta: {
      title: 'Source report'
    },
    path: '/source-report',
    name: 'source-report',    
    component: () => import('@/views/SourceReport.vue')
  }, 
  {
    meta: {
      title: 'Profile'
    },
    path: '/profile',
    name: 'profile',
    component: () => import('@/views/Profile.vue')
  },  
  {
    path: '/full-page',
    component: () => import(/* webpackChunkName: "full-page" */ '@/views/FullPage.vue'),
    children: [
      {
        meta: {
          title: 'Login'
        },
        path: '/login',
        name: 'login',
        component: () => import(/* webpackChunkName: "full-page" */ '@/views/full-page/Login.vue')
      }
    ]
  }

]

const router = new VueRouter({
  routes,
  scrollBehavior (to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    } else {
      return { x: 0, y: 0 }
    }
  }
})

export default router

export function useRouter () {
  return router
}
