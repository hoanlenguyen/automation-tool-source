<template>
  <section class="section is-main-section">
    <h5 class="subtitle is-6 mb-3">
      Total number of leads: <strong>{{ totalLeads | formattedNumber }}</strong>
    </h5>
    <b-field grouped class="">
      <div class="mr-3">
        <p class="subtitle is-6 mt-3">For Total Points above</p>
      </div>
      <b-field>
        <b-input v-model="totalPointsFrom" type="number" clearable></b-input>
      </b-field>
      <b-button
        label="Search"
        type="is-link"
        class="mr-3"
        icon-left="magnify"
        @click="getTotalCountByRange"
        :loading="isLoading"
      />
    </b-field>
    <h5 class="subtitle is-6">
      Total number of data: <strong>{{ totalData | formattedNumber }}</strong>
    </h5>
  </section>
</template>
<script>
import moment from "moment";
import { getTotalLeads, getTotalCountByRange } from "@/api/overallReport";
export default {
  name: "OverallReport",
  created() {
    this.getTotalLeads();
    //this.getTotalCountByRange();
  },
  data() {
    return {
      totalLeads: 0,
      totalData: null,
      totalPointsFrom: null,
      isLoading: false,
    };
  },
  watch: {},
  computed: {},
  methods: {
    getTotalCountByRange() {
      this.isLoading = true;
      getTotalCountByRange({ totalPointsFrom: this.totalPointsFrom })
        .then((response) => {
          if (response.status == 200) {
            this.totalData = response.data.totalCount;
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    getTotalLeads() {
      getTotalLeads()
        .then((response) => {
          if (response.status == 200) {
            this.totalLeads = response.data.totalCount;
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          //this.isLoading = false;
        });
    },
  },
};
</script>
