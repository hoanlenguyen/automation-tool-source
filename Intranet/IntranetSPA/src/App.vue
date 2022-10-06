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
import NavBar from '@/components/NavBar.vue'
import AsideMenu from '@/components/AsideMenu.vue'

export default {
  name: 'Home',
  components: {
    //FooterBar,
    AsideMenu,
    NavBar
  },
  data () {
    return {}
  },
  created () {
  },
  computed: {
    currentRouteName() {
        return [this.$route.meta.title];
    },
    filteredMenu(){
      var items=[];
      var settings=[];
      if(!this.$store.state.userPermissions)
        return [items];

      if(this.$store.state.userPermissions.includes(
        "Employee.View"
      ))
        items.push(  {
        to: '/employee',
        label: 'Employees',
        icon: 'account-multiple'
        });

      if(this.$store.state.userPermissions.includes(
        "TimeOff.View"
      ))
        items.push(  {
        to: '/time-off',
        label: 'Time-off',
        icon: 'clock-outline'
        });
      
      if(this.$store.state.userPermissions.includes(
        "LeaveHistory.View"
      ))
        items.push({
        to: '/leave-history',
        label: 'Leave history',
        icon: 'clipboard-text'
      });
      
      if(this.$store.state.userPermissions.includes(
        "Currency.View"
      ))
        settings.push({
          to: '/currency',
          label: 'Currencies',
          icon: 'currency-usd'
        });


      if(this.$store.state.userPermissions.includes(
        "Bank.View"
      ))
        settings.push({
          to: '/bank',
          label: 'Banks',
          icon: 'bank'
        });

      if(this.$store.state.userPermissions.includes(
        "Brand.View"
      ))
      settings.push({
        to: '/brand',
        label: 'Brands',
        icon: 'watermark'
        });

      if(this.$store.state.userPermissions.includes(
        "Department.View"
      ))
        settings.push({
        to: '/department',
        label: 'Departments',
        icon: 'domain'
        });

      if(this.$store.state.userPermissions.includes(
        "Rank.View"
      ))
        settings.push({
          to: '/rank',
          label: 'Ranks',
          icon: 'account-supervisor'
        });

      if(this.$store.state.userPermissions.includes(
        "Role.View"
      ))
        settings.push( {
        to: '/role',
        label: 'User rights',
        icon: 'shield-account'
        });

      if(settings.length>0)
        items.push(
        {
          label: 'Settings',
          subLabel: 'Settings',
          icon: 'format-list-bulleted',
          menu: settings
        }
      )

       return [items];
    },
 }
  
}
</script>
