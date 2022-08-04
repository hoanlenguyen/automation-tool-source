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
      if(!this.$store.state.userPermissions)
        return [items];

      if(this.$store.state.userPermissions.includes(
        "Permissions.Bank.View"
      ))
        items.push({
          to: '/bank',
          label: 'Banks',
          icon: 'bank'
        });

      if(this.$store.state.userPermissions.includes(
        "Permissions.Brand.View"
      ))
        items.push({
        to: '/brand',
        label: 'Brands',
        icon: 'watermark'
        });

      if(this.$store.state.userPermissions.includes(
        "Permissions.Department.View"
      ))
        items.push({
        to: '/department',
        label: 'Departments',
        icon: 'domain'
        });

      if(this.$store.state.userPermissions.includes(
        "Permissions.Rank.View"
      ))
        items.push({
          to: '/rank',
          label: 'Ranks',
          icon: 'shield-account'
        });

      if(this.$store.state.userPermissions.includes(
        "Permissions.Role.View"
      ))
        items.push( {
        to: '/role',
        label: 'User rights',
        icon: 'shield-account'
        });

      if(this.$store.state.userPermissions.includes(
        "Permissions.Employee.View"
      ))
        items.push(  {
        to: '/employee',
        label: 'Employees',
        icon: 'account-multiple'
        });

       return [items];
    },
 }
  
}
</script>
