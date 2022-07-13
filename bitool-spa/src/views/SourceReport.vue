<template>
  <section class="section is-main-section">
    <b-table :data="data" :loading="isLoading">      
      <b-table-column
        field="Source"
        label="Source"
        width="300px">
        <template v-slot="props">{{ props.row.source }}</template>        
      </b-table-column>

      <b-table-column
        field="TotalNumbers"
        label="Total numbers"
        v-slot="props"
        width="400px">
        {{props.row.totalNumbers|formattedNumber}}
      </b-table-column>

      <b-table-column
        field="TotalPoints"
        label="Overall Data score average"
        v-slot="props"
        width="400px"
      >
      <span>{{props.row.averagePoints | roundNumber}}</span>
      <!-- <span v-if="props.row.totalNumbers>0">{{props.row.totalPoints/props.row.totalNumbers|roundNumber}}</span> -->
      </b-table-column>
      
     <!-- <template #empty>
        <div class="has-text-centered">No records</div>
      </template> -->

     <!--  <div slot="footer" class="is-flex 
        is-flex-direction-row
        is-align-items-center
        is-flex-wrap-wrap">
        <b-button
          label="Reset"
          type="is-light"
          class="mr-4"
          icon-left="reload"
          @click="resetFilter"
        /> 
      </div>-->
    </b-table>
  </section>
</template>
<script>
import { getSummary  } from "@/api/sourceReport";
export default {
  name: "SourceReport",
  created() {
    this.getSummary();
  },
  data() {
    return {
      data: [],
      totalItems:0,
      isLoading:false,
      //totalTablePoints:0      
    };
  },
  watch: {},
  computed: {
    
  },
  methods: {
    resetFilter() {      
      this.getSummary();
    },
    getSummary() {
      this.isLoading = true;
      getSummary()
        .then((response) => {
          if (response.status == 200) {
            this.data = response.data;
            //this.totalTablePoints= this.data.map(item => item.totalPoints).reduce((prev, next) => prev + next);
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    }
  }
};
</script>
