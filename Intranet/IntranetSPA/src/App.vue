<template>
  <div id="app">
    <nav-bar :title-stack="currentRouteName" />
    <aside-menu :menu="filteredMenu" />
    <router-view />
    <!-- <footer-bar /> -->
    <!-- {{filteredMenu}} -->
  </div>
</template>

<script>
import menu from '@/menu.js'
import NavBar from '@/components/NavBar.vue'
import AsideMenu from '@/components/AsideMenu.vue'
// import FooterBar from '@/components/FooterBar.vue'

export default {
  name: 'Home',
  components: {
    //FooterBar,
    AsideMenu,
    NavBar
  },
  data () {
    return {
      menu
    }
  },
  created () {
    // this.$store.commit('user', {
    //   name: 'John Doe',
    //   email: 'john@example.com',
    //   avatar: 'https://avatars.dicebear.com/v2/gridy/John-Doe.svg'
    // })
  },
  computed: {
    currentRouteName() {
        return [this.$route.meta.title];
    },
    filteredMenu(){
      var items=[];
      if(this.$store.state.userPermissions &&
      this.$store.state.userPermissions.includes(
        "Permissions.Bank.View"
      ))
      items.push({
        to: '/bank',
        label: 'Banks',
        icon: 'bank'
      });
      //return items;
       return [items];
    },
 }
  
}
</script>
