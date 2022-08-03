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
      title: 'Role data'
    },
    path: '/role',
    name: 'role',    
    component: () => import('@/views/Role.vue')
  },
  {
    meta: {
      title: 'Bank data'
    },
    path: '/bank',
    name: 'bank',    
    component: () => import('@/views/Bank.vue')
  },
  {
    meta: {
      title: 'Brand data'
    },
    path: '/brand',
    name: 'brand',    
    component: () => import('@/views/Brand.vue')
  },
  {
    meta: {
      title: 'Department data'
    },
    path: '/department',
    name: 'department',    
    component: () => import('@/views/Department.vue')
  },
  {
    meta: {
      title: 'Rank data'
    },
    path: '/rank',
    name: 'rank',    
    component: () => import('@/views/Rank.vue')
  },
  {
    meta: {
      title: 'Employee'
    },
    path: '/employee',
    name: 'Employee',    
    component: () => import('@/views/Employee.vue')
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
