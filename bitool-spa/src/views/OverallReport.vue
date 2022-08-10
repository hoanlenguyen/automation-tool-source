<template>
  <section class="section is-main-section">
    <h5 class="subtitle is-6 mb-3">
      Total number of leads: <strong>{{ totalLeads | formattedNumber }}</strong>
    </h5>
    <b-table
        :data="totalPointRanges"
        bordered
        narrowed
        hoverable
        :loading="isLoading" style="width:300px">

        <b-table-column field="totalPoints" label="Points" width="40" v-slot="props">
          {{ props.row.totalPoints }}
        </b-table-column>

        <b-table-column field="count" label="Total Numbers"  width="40" numeric v-slot="props">
          {{ props.row.count |formattedNumber }}
        </b-table-column>
      </b-table>
    <!-- <b-field grouped class="">
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
    </h5> -->
  </section>
</template>
<script>
//import moment from "moment";
import { getTotalLeads, getTotalCountByRange, getTotalCountByLimitedRange } from "@/api/overallReport";
export default {
  name: "OverallReport",
  created() {
    this.getTotalCountByLimitedRange(20);
    //this.getTotalCountByRange();
  },
  data() {
    return {
      totalLeads: 0,
      totalPointRanges:[],
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
    getTotalCountByLimitedRange(value) {
      this.isLoading = true;
      getTotalCountByLimitedRange(value)
        .then((response) => {
          if (response.status == 200) {
            this.totalPointRanges = response.data;
            this.totalLeads= this.totalPointRanges.map(item => item.count).reduce((prev, next) => prev + next);
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
  },
};
</script>
