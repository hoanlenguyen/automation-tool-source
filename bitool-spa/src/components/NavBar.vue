<template>
  <nav v-show="isNavBarVisible" id="navbar-main" class="navbar is-fixed-top">
    <div class="navbar-brand">
      <a
        class="navbar-item is-hidden-desktop"
        @click.prevent="asideToggleMobile"
      >
        <b-icon :icon="asideMobileIcon" />
      </a>
      <a
        class="navbar-item is-hidden-touch is-hidden-widescreen is-desktop-icon-only"
        @click.prevent="asideDesktopOnlyToggle"
      >
        <b-icon icon="menu" />
      </a>
      <div
        class="navbar-item has-control no-left-space-touch no-left-space-desktop-only"
      >
        <div class="level">
          <div class="level-left">
            <div class="level-item">
              <ul>
                <li
                  v-for="(title, index) in titleStack"
                  :key="index"
                  class="is-size-3"
                >
                  {{ title }}
                </li>
              </ul>
            </div>
            <!--  <div class="level-item">
              <b-message 
                size="is-small"
                type="is-success"
                v-model="isActiveMessage"  
                aria-close-label="Close message">
                <a title="Close"  @click="isActiveMessage=false">
                  <b-icon icon="close" size="is-small" type="is-primary"></b-icon> 
                </a>                
                <span v-html="importMessage.content"></span>
              </b-message>
            </div> -->
          </div>
        </div>
      </div>
    </div>
    <div class="navbar-brand is-right"></div>
      <a
        class="navbar-item navbar-item-menu-toggle is-hidden-desktop"
        @click.prevent="menuToggle"
      >
        <b-icon :icon="menuToggleIcon" custom-size="default" />
      </a>
    </div>
    <div
      class="navbar-menu fadeIn animated faster"
      :class="{ 'is-active': isMenuActive }"
    >    
      <div class="navbar-end">  
      <!--<p>notificationMessages {{$store.state.notificationMessages.length}}</p>-->
      <!--<p>notificationMessages {{$store.state.notificationMessages}}</p>  -->
      <!--<p>signalRConnectionId {{$store.state.signalRConnectionId}}</p>  -->
        <b-dropdown aria-role="list"  position="is-bottom-left" trap-focus expanded :max-height="250" :scrollable="true" style="min-width: 500px; text-align: right;">
          <template #trigger>
            <b-icon class="mt-4" :icon="unReadNotiCount? `bell-ring-outline`: `bell-outline`" :type="unReadNotiCount? `is-primary`: `is-dark`"></b-icon>
            <span v-if="unReadNotiCount" class="mt-0 pt-0 mb-5 pb-5"><b-tag rounded type="is-primary" style="width:14px; height:14px; font-size: 10px; border-radius: 50%;" >{{unReadNotiCount}}</b-tag></span>
          </template>
          <b-dropdown-item :expanded="true" custom :focusable="false" paddingless aria-role="listitem" v-for="(message, index) in $store.state.notificationMessages" class="py-1" style="text-align: left;">
            <b-message size="is-small" :type="!message.isRead?`is-success`: `is-dark`" class="m-0 p-0" :style="message.isRead?`cursor: context-menu`: `cursor: pointer`" @click.native="readNotificationMessages(index)">
              <div class="columns">
                <div class="column is-11"><span v-html="message.content"></span></div>
                <div class="column is-1"><b-icon v-if="!message.isRead" icon="circle-small" type="is-info" style="height: 100%;"></b-icon></div>
              </div>                          
            </b-message>
          </b-dropdown-item>                    
        </b-dropdown>

        <nav-bar-menu class="has-divider has-user-avatar">
          <span class="mr-2">Hi,</span>
          <div class="is-user-name">
            <span>{{ userName }}</span>
          </div>
         <div slot="dropdown" class="navbar-dropdown">
            <router-link
              to="/profile"
              class="navbar-item"
              exact-active-class="is-active"
            >
              <b-icon icon="account" custom-size="default" />
              <span>My Profile</span>
            </router-link>           
          </div> 
        </nav-bar-menu>
        <router-link
          to="/login"
          class="navbar-item"
          exact-active-class="is-active"
        >
          <a
            class="navbar-item is-desktop-icon-only"
            title="Log out"
            @click="logout"
          >
            <b-icon icon="logout" custom-size="default" />
            <span>Log out</span>
          </a>
        </router-link>
      </div>
    </div>
  </nav>
</template>

<script>
import { mapState } from "vuex";
import NavBarMenu from "@/components/NavBarMenu.vue";
import {setToken} from '@/utils/auth';
//import UserAvatar from "@/components/UserAvatar.vue";

export default {
  name: "NavBar",
  components: {
    //UserAvatar,
    NavBarMenu,
  },
  props: {
    titleStack: {
      type: Array,
      default: () => [],
    },
  },
  created() {
    this.$socket.start({
      log: true // Logging is optional but very helpful during development
    });

    const that = this;
    this.$socket.on('GetConnectionId', function (data) {
    });
    this.$socket.on('CompleteImport', function (data) {     
    });
    this.$socket.on('CompleteAssignCampaign', function (data) {
    });
    if(!this.$store.state.signalRConnectionId)
      this.$socket.invoke('GetCurrentConnectionId').then((data) => {console.log('invoke'+ data);})
  },  
  sockets: {
    GetConnectionId(data){
      //this.connectionId = data;
      this.$store.commit('signalR', data);
    },
    CompleteImport(data){
      console.log(data);
      //this.isActiveMessage=true;
      //this.importMessage = data;
      this.$store.commit('notificationMessages',data);
    },
    CompleteAssignCampaign(data){
      //this.isActiveMessage=true;
      //this.importMessage = data;
      this.$store.commit('notificationMessages', data);
    }
  },
  data() {
    return {
      //connectionId:null,
      isMenuActive: false,
      //isActiveMessage:false,
      // importMessage:{
      //   title: null,
      //   content: null
      // }
    };
  },
  computed: {
    unReadNotiCount(){
      if(this.$store.state.notificationMessages.length==0) return 0;
      return this.$store.state.notificationMessages.filter(p=>p.isRead!==true).length;
    },
    asideMobileIcon() {
      return this.isAsideMobileExpanded ? "backburger" : "forwardburger";
    },
    menuToggleIcon() {
      return this.isMenuActive ? "close" : "dots-vertical";
    },
    ...mapState(["isAsideMobileExpanded", "isNavBarVisible", "userName"]),
  },
  mounted() {
    this.$router.afterEach(() => {
      this.isMenuActive = false;
    });
  },
  methods: {
    readNotificationMessages(index){
      console.log('index' + index);
      this.$store.commit("readNotificationMessages", index);
    },
    asideToggleMobile() {
      this.$store.commit("asideMobileStateToggle");
    },
    asideDesktopOnlyToggle() {
      this.$store.dispatch("asideDesktopOnlyToggle");
    },
    menuToggle() {
      this.isMenuActive = !this.isMenuActive;
    },
    logout() {
      setToken('');
      sessionStorage.clear();
      this.$store.commit('user',  {});
      this.$store.commit('clearNotificationMessages');
      this.$store.commit('clearSignalRConnectionId');
      this.$router.push({ name: 'login' });
    },
  },
};
</script>
