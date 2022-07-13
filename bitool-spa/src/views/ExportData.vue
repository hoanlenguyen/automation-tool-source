<template>
  <div>
    <section class="section is-main-section">
      <div class="columns">
        <div class="column is-11  m-0 pb-0">
          <b-field grouped>
            <div class="mr-3">
              <p class="title is-6">Customer</p>
              <p class="subtitle is-6">Date First Added from</p>
            </div>

            <b-field>
              <b-datepicker
                icon="calendar-today"
                locale="en-CA"
                v-model="dateFirstAddedFrom"
                editable
              >
              </b-datepicker>
            </b-field>
            <div class="mr-3">
              <p class="title is-6 has-text-white-bis">.</p>
              <p class="subtitle is-6">to</p>
            </div>
            <b-field>
              <b-datepicker
                icon="calendar-today"
                locale="en-CA"
                v-model="dateFirstAddedTo"
                editable
              >
              </b-datepicker>
            </b-field>
            <div>
              <p>
                <b-radio v-model="filter.sortingValue" size="is-small" native-value="DateFirstAdded asc">
                  ASC 
                </b-radio>
              </p>
              <p>
                <b-radio v-model="filter.sortingValue" size="is-small" native-value="DateFirstAdded desc">
                  DESC
                </b-radio>
              </p>
            </div>        
          </b-field>
        </div>
        <div class="column  m-0 pb-0">
          <b-button           
            class="button"
            @click="isImageModalActive = true" 
            style="padding: 0; border: none; background: none;">
            <b-icon
              icon="file-image-outline"
              size="is-medium"
              type="is-info">
            </b-icon>
          </b-button>
        </div>
      </div>

      <b-field grouped class="mb-3">
        <div class="mr-3">
          <p class="title is-6">Campaigns</p>
          <p class="subtitle is-6">Total Times Exported from</p>
        </div>
        <b-field>
          <b-input v-model="filter.totalTimesExportedFrom" type="number"></b-input>
        </b-field>
        <div class="mr-3">
          <p class="title is-6 has-text-white-bis">.</p>
          <p class="subtitle is-6">to</p>
        </div>
        <b-field>
          <b-input v-model="filter.totalTimesExportedTo" type="number"></b-input>
        </b-field>       
      </b-field>

      <b-field grouped class="mb-3">
        <div class="mr-3">
          <p class="subtitle is-6 pt-3">Date Last Export from</p>
        </div>
        <b-field>
          <b-datepicker
            icon="calendar-today"
            locale="en-CA"
            v-model="dateLastExportedFrom"
            editable
          >
          </b-datepicker>
        </b-field>
        <div class="mr-3">
          <p class="subtitle is-6 pt-3">to</p>
        </div>
        <b-field>
          <b-datepicker
            icon="calendar-today"
            locale="en-CA"
            v-model="dateLastExportedTo"
            editable
          >
          </b-datepicker>
        </b-field> 
      </b-field>

      <b-field grouped class="mb-3">
        <div class="mr-3">
          <p class="subtitle is-6 pt-3">Last 3 Campaigns Used</p>
        </div>
        <b-field>
          <multiselect
            v-model="selectLast3CampaignsUsed"
            tag-placeholder=""
            placeholder="Select campaigns"
            label="campaignName"
            track-by="campaignID"
            :options="adminCampaigns"
            :multiple="true"
            :max="3"
            :taggable="true"
            selectLabel="Add"
            deselectLabel="Remove"
          >
          <template slot="maxElements" >
            <p class="subtitle is-6 has-text-danger my-2">Maximum of 3 options selected.</p>
            <p class="subtitle is-6 has-text-danger my-2">First remove a selected option to select another.</p>
          </template>
          </multiselect>
        </b-field>
      </b-field>

      <b-field grouped class="mb-3">
        <div class="mr-3">
          <p class="title is-6">Occurrence (Indicators)</p>
          <p class="subtitle is-6">Date Last Occurred from</p>
        </div>
        <b-field>
          <b-datepicker
            icon="calendar-today"
            locale="en-CA"
            v-model="dateLastOccurredFrom"
            editable
          >
          </b-datepicker>
        </b-field>
        <div class="mr-3">
          <p class="title is-6 has-text-white-bis">.</p>
          <p class="subtitle is-6">to</p>
        </div>
        <b-field>
          <b-datepicker
            icon="calendar-today"
            locale="en-CA"
            v-model="dateLastOccurredTo"
            editable
          >
          </b-datepicker>
        </b-field>         
      </b-field>

      <b-field grouped class="mb-3">
        <div class="mr-3">
          <p class="subtitle is-6 mt-3">Occurred Categories</p>
        </div>
        <b-field>
          <multiselect
            v-model="selectOccurredCategories"
            tag-placeholder=""
            placeholder="Select Occurred Categories"
            label="scoreTitle"
            track-by="scoreID"
            :options="occurredCategories"
            :multiple="true"
            :taggable="true"
            selectLabel="Add"
            deselectLabel="Remove"
          >
          </multiselect>
        </b-field>
      </b-field>

      <b-field grouped class="mb-3">
        <div class="mr-3">
          <p class="subtitle is-6 mt-3">Total Occurrence Points from</p>
        </div>
        <b-field>
          <b-input v-model="filter.totalOccurancePointsFrom" type="number"></b-input>
        </b-field>
        <div class="mr-3">
          <p class="subtitle is-6 mt-3">to</p>
        </div>
        <b-field>
          <b-input v-model="filter.totalOccurancePointsTo" type="number"></b-input>
        </b-field>            
      </b-field>
      
      <b-field grouped class="mb-3">
        <div class="mr-3">
          <p class="title is-6">Results</p>
          <p class="subtitle is-6">Results Categories</p>
        </div>
        <b-field>
          <multiselect
            v-model="selectResultsCategories"
            tag-placeholder=""
            placeholder="Select Results Categories"
            label="scoreTitle"
            track-by="scoreID"
            :options="resultsCategories"
            :multiple="true"
            :taggable="true"
            selectLabel="Add"
            deselectLabel="Remove"
          >
          </multiselect>
        </b-field>
      </b-field>

      <b-field grouped class="mb-3">
        <div class="mr-3">
          <p class="subtitle is-6 mt-3">Total Results Points from</p>
        </div>
        <b-field>
          <b-input v-model="filter.totalResultsPointsFrom" type="number"></b-input>
        </b-field>
        <div class="mr-3">
          <p class="subtitle is-6 mt-3">to</p>
        </div>
        <b-field>
          <b-input v-model="filter.totalResultsPointsTo" type="number"></b-input>
        </b-field>            
      </b-field>

      <b-field grouped class="mb-3">
        <div class="mr-3">
          <p class="title is-6">Analysis</p>
          <p class="subtitle is-6">Total Points from</p>
        </div>
        <b-field>
          <b-input v-model="filter.totalPointsFrom" type="number"></b-input>
        </b-field>
         <div class="mr-3">
          <p class="subtitle is-6 mt-3">to</p>
        </div>
        <b-field>
          <b-input v-model="filter.totalPointsTo" type="number"></b-input>
        </b-field>
      </b-field>

      <b-field grouped class="mb-3">
        <div class="mr-3">
          <p class="subtitle is-6 mt-3">Export vs Points (%) from</p>
        </div>
        <b-field>
          <b-input v-model="filter.exportVsPointsPercentageFrom" type="number"></b-input>
        </b-field>
         <div class="mr-3">
          <p class="subtitle is-6 mt-3">to</p>
        </div>
        <b-field>
          <b-input v-model="filter.exportVsPointsPercentageTo" type="number"></b-input>
        </b-field>        
      </b-field>

      <b-field grouped class="mb-3">
        <div class="mr-3">
          <p class="subtitle is-6 mt-3">Export vs Points Exceptions</p>
        </div>
        <b-field>
          <multiselect
            v-model="selectExportVsPointsExceptions"
            tag-placeholder=""
            placeholder="Select exceptions"             
            :options="exportVsPointsExceptions"
            :multiple="true"
            :taggable="true"
            selectLabel="Add"
            deselectLabel="Remove"
          >
          </multiselect>
        </b-field>
        
      </b-field>

      <b-field grouped class="mb-3">
        <div class="mr-3">
          <p class="subtitle is-6 mt-3">Export vs Points (in Points) from</p>
        </div>
        <b-field>
          <b-input v-model="filter.exportVsPointsNumberFrom" type="number"></b-input>
        </b-field>
         <div class="mr-3">
          <p class="subtitle is-6 mt-3">to</p>
        </div>
        <b-field>
          <b-input v-model="filter.exportVsPointsNumberTo" type="number"></b-input>
        </b-field>  
      </b-field>

      <b-field grouped class="mb-3">
        <div class="mr-3">
          <p class="title is-6">Others</p>
          <p class="subtitle is-6">Export top</p>
        </div>
        <b-field>
          <b-input v-model="filter.exportTop" type="number"></b-input> <span class="ml-3 mt-3">numbers</span>
        </b-field>        
      </b-field>      
      <h5 class="subtitle is-6 mb-1">
        <span  v-show="isShowResult">Found {{ formattedTotalCount }} customer mobile numbers</span>
        <span v-show="!isShowResult" class="has-text-white-bis">.</span>
      </h5>

      <b-field grouped>
        <p class="control">
          <b-button
            label="Search"
            type="is-link"
            class="mr-3"
            icon-left="magnify"
            @click="getCustomerCount"
            :loading="isLoading"
          />
          <b-button
            label="Reset"
            type="is-light"
            class="mr-3"
            icon-left="reload"
            @click="resetFilter"
          />
          <b-button
            label="Download result"
            class="mr-3"
            type="is-primary"
            @click="downloadCustomerList"
            :disabled="isDisableDownload"
            :loading="isLoadingDownload"
          />

          <b-button
            label="Assign campaign"
            type="is-primary"
            @click="isShowCampaign = !isShowCampaign"
            :disabled="isDisableDownload"
          />
        </p>
        <b-field
          class="control"
          v-show="isShowCampaign"
        >
          <b-select
            placeholder="Select campaign"
            v-model="filter.campaignID"
            clearable
          >
            <option
              v-for="option in adminCampaigns"
              :value="option.campaignID"
              :key="option.campaignID"
            >
              {{ option.campaignName }}
            </option>
          </b-select>
          <b-button
            label="Confirm"
            type="is-primary"
            @click="assignCampaignToCustomers"
            :disabled="!filter.campaignID"
            :loading="isConfirmingCampaign"
          />
        </b-field>
      </b-field>
        <b-modal v-model="isImageModalActive"  :width="`100%`" scroll="keep">
          <figure  class="image is-3by1 is-fullwidth">
            <img src="../assets/lead_management_report.png">
          </figure>
      </b-modal>
    </section>
  </div>
</template>

<script>
import moment from "moment";
import Multiselect from "vue-multiselect";
import { saveAs } from 'file-saver';
import { getAdminScores, getAdminCampaigns } from "@/api/importData";
import { getCustomerCount, assignCampaignToCustomers, downloadCustomersBySP } from "@/api/exportData";
export default {
  name: "ExportData",
  components: { Multiselect },
  created() {
    this.getAdminScoreList();
    this.getAdminCampaignList();
  },
  data() {
    return {
      adminScores: [],
      adminCampaigns: [],
      occurredCategories:[],
      resultsCategories:[],
      exportVsPointsExceptions:["No Occurance","No Export"],
      selectOccurredCategories:[],
      selectResultsCategories:[],
      selectExportVsPointsExceptions:[],
      selectLast3CampaignsUsed:[],
      filter: {
        dateFirstAddedFrom: null,
        dateFirstAddedTo: null,

        totalTimesExportedFrom: null,
        totalTimesExportedTo: null,
        
        dateLastExportedFrom: null,
        dateLastExportedTo: null,

        last3CampaignsUsed:null,

        dateLastOccurredFrom:null,
        dateLastOccurredTo:null,

        occurredCategories:null,
        
        totalOccurancePointsFrom:null,
        totalOccurancePointsTo:null,

        resultsCategories:null,

        totalResultsPointsFrom:null,
        totalResultsPointsTo:null,

        totalPointsFrom:null,
        totalPointsTo:null,

        exportVsPointsPercentageFrom:null,
        exportVsPointsPercentageTo:null,

        exportVsPointsExceptions:null,

        exportVsPointsNumberFrom:null,
        exportVsPointsNumberTo:null,

        sortBy:null,
        sortDirection:null,
        sortingValue:null,
        exportTop:null,

        campaignID: null,
        totalCount: 0
      },
      defaultFilter: {
        dateFirstAddedFrom: null,
        dateFirstAddedTo: null,

        totalTimesExportedFrom: null,
        totalTimesExportedTo: null,
        
        dateLastExportedFrom: null,
        dateLastExportedTo: null,

        last3CampaignsUsed:null,

        dateLastOccurredFrom:null,
        dateLastOccurredTo:null,

        occurredCategories:null,
        
        totalOccurancePointsFrom:null,
        totalOccurancePointsTo:null,

        resultsCategories:null,

        totalResultsPointsFrom:null,
        totalResultsPointsTo:null,

        totalPointsFrom:null,
        totalPointsTo:null,

        exportVsPointsPercentageFrom:null,
        exportVsPointsPercentageTo:null,

        exportVsPointsExceptions:null,

        exportVsPointsNumberFrom:null,
        exportVsPointsNumberTo:null,

        sortBy:null,
        sortDirection:null,
        sortingValue:null,
        exportTop:null,

        campaignID: null,
        totalCount: 0
      },
      dateFirstAddedFrom: null,
      dateFirstAddedTo: null,   
      dateLastExportedFrom: null,
      dateLastExportedTo: null,
      dateLastOccurredFrom: null,
      dateLastOccurredTo: null,         
      customerList: [],
      totalCount: 0,
      isShowResult: false,
      isLoading: false,
      isLoadingDownload: false,
      isShowCampaign: false,
      isConfirmingCampaign: false,
      isImageModalActive:false
    };
  },
  computed: {
    isDisableDownload() {
      return this.totalCount == 0;
    },
    formattedTotalCount(){
      if(!this.totalCount) return '0';
      return this.totalCount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
  },
  methods: {
    resetFilter() {
      this.filter = { ...this.defaultFilter };
      this.totalCount=0;
      this.customerList = [];
      this.dateFirstAddedFrom = null;
      this.dateFirstAddedTo = null;
      this.dateLastExportedFrom = null;
      this.dateLastExportedTo = null;
      this.isShowResult = false;
      this.isShowCampaign = false;
      this.selectLast3CampaignsUsed=[];
      this.selectOccurredCategories=[];
      this.selectResultsCategories=[];
      this.selectExportVsPointsExceptions=[];
    },
    getAdminScoreList() {
      //this.isLoading = true;
      getAdminScores()
        .then((response) => {
          if (response.status == 200) {
            this.adminScores = response.data;
            this.occurredCategories= this.adminScores.filter(p=>p.scoreCategory=='Occurance');
            this.resultsCategories=this.adminScores.filter(p=>p.scoreCategory=='Results');
            //console.log(this.adminScores);
          } else if (response.status == 401) {
            this.$router.push({ name: "login" });
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          //this.isLoading = false;
        });
    },
    getAdminCampaignList() {
      //this.isLoading = true;
      getAdminCampaigns()
        .then((response) => {
          if (response.status == 200) {
            this.adminCampaigns = response.data;
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          //this.isLoading = false;
        });
    },
    getCustomerCount() {
      this.isLoading = true;
      const outputFormat = "YYYY-MM-DD";
      this.filter.dateFirstAddedFrom =this.dateFirstAddedFrom? moment(this.dateFirstAddedFrom).format(outputFormat):null;
      this.filter.dateFirstAddedTo =this.dateFirstAddedTo? moment(this.dateFirstAddedTo).format(outputFormat):null;

      this.filter.dateLastExportedFrom =this.dateLastExportedFrom? moment(this.dateLastExportedFrom).format(outputFormat):null;
      this.filter.dateLastExportedTo =this.dateLastExportedTo? moment(this.dateLastExportedTo).format(outputFormat):null;

      this.filter.dateLastOccurredFrom =this.dateLastOccurredFrom? moment(this.dateLastOccurredFrom).format(outputFormat):null;
      this.filter.dateLastOccurredTo =this.dateLastOccurredTo? moment(this.dateLastOccurredTo).format(outputFormat):null;      

      this.filter.last3CampaignsUsed =this.selectLast3CampaignsUsed.length>0?
        this.selectLast3CampaignsUsed.map((p) => p.campaignID).join() : null;

      this.filter.occurredCategories = this.selectOccurredCategories.length>0?
        this.selectOccurredCategories.map((p) => p.scoreID).join(): null;
      
      this.filter.resultsCategories =this.selectResultsCategories.length>0?
        this.selectResultsCategories.map((p) => p.scoreID).join(): null;
      
      this.filter.exportVsPointsExceptions =this.selectExportVsPointsExceptions.length>0?
        this.selectExportVsPointsExceptions.join(): null;
      
      if(this.filter.sortingValue){
        const sortingValue = this.filter.sortingValue.split(" ");
        this.filter.sortBy=sortingValue[0];
        this.filter.sortDirection=sortingValue[1];
      }else{
        this.filter.sortBy=null;
        this.filter.sortDirection=null;
      }

      if(!this.filter.exportTop ||isNaN(this.filter.exportTop))
        this.filter.exportTop=null;

      getCustomerCount(this.filter)
        .then((response) => {
          if (response.status == 200 && response.data) {
            const data=  response.data;
            this.filter.totalCount=  this.totalCount= data.totalCount;
            //this.customerList = response.data;
            //console.log(this.customerList);
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          this.isShowResult = true;
          this.isLoading = false;
          this.isShowCampaign = false;
        });
    },
    downloadCustomerList() {
      this.isLoadingDownload = true;
      const outputFormat = "YYYY-MM-DD";
      this.filter.dateFirstAddedFrom =this.dateFirstAddedFrom? moment(this.dateFirstAddedFrom).format(outputFormat):null;
      this.filter.dateFirstAddedTo =this.dateFirstAddedTo? moment(this.dateFirstAddedTo).format(outputFormat):null;

      this.filter.dateLastExportedFrom =this.dateLastExportedFrom? moment(this.dateLastExportedFrom).format(outputFormat):null;
      this.filter.dateLastExportedTo =this.dateLastExportedTo? moment(this.dateLastExportedTo).format(outputFormat):null;

      this.filter.dateLastOccurredFrom =this.dateLastOccurredFrom? moment(this.dateLastOccurredFrom).format(outputFormat):null;
      this.filter.dateLastOccurredTo =this.dateLastOccurredTo? moment(this.dateLastOccurredTo).format(outputFormat):null;      

      this.filter.last3CampaignsUsed =this.selectLast3CampaignsUsed.length>0?
        this.selectLast3CampaignsUsed.map((p) => p.campaignID).join() : null;

      this.filter.occurredCategories = this.selectOccurredCategories.length>0?
        this.selectOccurredCategories.map((p) => p.scoreID).join(): null;
      
      this.filter.resultsCategories =this.selectResultsCategories.length>0?
        this.selectResultsCategories.map((p) => p.scoreID).join(): null;
      
      this.filter.exportVsPointsExceptions =this.selectExportVsPointsExceptions.length>0?
        this.selectExportVsPointsExceptions.join(): null;
      
      if(this.filter.sortingValue){
        const sortingValue = this.filter.sortingValue.split(" ");
        this.filter.sortBy=sortingValue[0];
        this.filter.sortDirection=sortingValue[1];
      }else{
        this.filter.sortBy=null;
        this.filter.sortDirection=null;
      }

      if(!this.filter.exportTop ||isNaN(this.filter.exportTop))
        this.filter.exportTop=null;

      downloadCustomersBySP(this.filter)
        .then((response) => {
          if (response.status == 200 && response.data) {
            const data =  response.data;
            if(data.result){
              const result=data.result;
              for (let i = 0; i < result.length; i++) {   
                let filename = result[i].substring(result[i].lastIndexOf('/')+1);
                let fileUrl= result[i];
                setTimeout(function() {          
                  console.log(filename);
                  saveAs(fileUrl, filename); },200);
              }
              this.isLoadingDownload=false;
            } 
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          this.isLoadingDownload=false;
        });
    },
    downloadCustomerExcel() {
      console.log("downloadCustomerExcel");
      if (this.totalCount > 0) {
        let mobileList = this.customerList.map((p) => ({
          CustomerMobileNo: p,
        }));
        this.exportExcelData(mobileList, "CustomerMobileNoList", 30, false);
      }
    },
    assignCampaignToCustomers() {
      this.isConfirmingCampaign = true;
      this.filter.signalRConnectionId= this.$store.state.signalRConnectionId;
      assignCampaignToCustomers(this.filter)
        .then((response) => {
          if (response.status == 200) {
            var data = response.data;
            if(!data.shouldSendEmail){
                 this.$buefy.snackbar.open({
                  message: `Assign campaign successfully!`,
                  queue: false,
                });
              }else{
                this.$buefy.snackbar.open({
                  message: `Assign campaign successfully!\nSystem will send email to inform when the new export data is ready`,
                  queue: false,
                  duration: 6000
                });
              }
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          // this.isShowResult = true;
          // this.isLoading = false;
          this.filter.campaignID = null;
          this.isConfirmingCampaign = false;
        });
    },
  },
};
</script>
<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>
