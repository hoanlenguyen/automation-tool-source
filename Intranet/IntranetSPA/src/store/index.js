import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'
import createPersistedState from 'vuex-persistedstate';
import Cookies from 'js-cookie';

Vue.use(Vuex)

const store = new Vuex.Store({
  state: {
    /* User */
    userName: null,
    userEmail: null,
    userAvatar: null,
    userRoleName:null,
    userPermissions:[],

    /* NavBar */
    isNavBarVisible: true,

    /* FooterBar */
    isFooterBarVisible: true,

    /* Aside */
    isAsideVisible: true,
    isAsideMobileExpanded: false,

    /* signalR */
    signalRConnectionId:null,

    notificationMessages:[],

    /* Sample data (commonly used) */
    clients: []
  },
  plugins: [createPersistedState({
    storage: {
      getItem: key => Cookies.get(key),
      setItem: (key, value) => Cookies.set(key, value, { expires: 3, secure: true }),
      removeItem: key => Cookies.remove(key)
    }
  })],
  mutations: {
    /* A fit-them-all commit */
    basic (state, payload) {
      state[payload.key] = payload.value
    },

    /* SignalR */
    signalR (state, payload) {
      if (payload) {
        state.signalRConnectionId = payload
      }
    },

    /* notificationMessages */
    notificationMessages (state, payload) {
      if (payload.content) {
        state.notificationMessages.unshift({content:payload.content, isRead:false})
      }
    },

    readNotificationMessages (state, payload) {
      if (!isNaN(payload)) {
        state.notificationMessages[payload].isRead=true;
      }
    },
    clearNotificationMessages(state, payload=null) {
      state.notificationMessages=[]
    },

    clearSignalRConnectionId(state, payload=null) {
      state.signalRConnectionId=null;
    },

    /* User */
    user (state, payload) {
      state.userName = payload.name;
      state.userEmail = payload.email;
      state.userAvatar = payload.avatar;
      state.userRoleName= payload.roleName;
      state.userPermissions= payload.permissions;
    },

    /* Aside Mobile */
    asideMobileStateToggle (state, payload = null) {
      const htmlClassName = 'has-aside-mobile-expanded'

      let isShow

      if (payload !== null) {
        isShow = payload
      } else {
        isShow = !state.isAsideMobileExpanded
      }

      if (isShow) {
        document.documentElement.classList.add(htmlClassName)
      } else {
        document.documentElement.classList.remove(htmlClassName)
      }

      state.isAsideMobileExpanded = isShow
    },

    /* Full Page mode */
    fullPage (state, payload) {
      state.isNavBarVisible = !payload
      state.isAsideVisible = !payload
      state.isFooterBarVisible = !payload
    }
  },
  actions: {
    asideDesktopOnlyToggle (store, payload = null) {
      let method

      switch (payload) {
        case true:
          method = 'add'
          break
        case false:
          method = 'remove'
          break
        default:
          method = 'toggle'
      }
      document.documentElement.classList[method]('has-aside-desktop-only-visible')
    },
    toggleFullPage ({ commit }, payload) {
      commit('fullPage', payload)

      document.documentElement.classList[!payload ? 'add' : 'remove']('has-aside-left', 'has-navbar-fixed-top')
    },
    fetch ({ commit }, payload) {
      axios
        .get(`data-sources/${payload}.json`)
        .then((r) => {
          if (r.data && r.data.data) {
            commit('basic', {
              key: payload,
              value: r.data.data
            })
          }
        })
        .catch(error => {
          alert(error.message)
        })
    },
    clearNotificationMessages (state, payload=null) {       
        state.notificationMessages=[]
    }    
  }
})

export default store

export function useStore () {
  return store
}
