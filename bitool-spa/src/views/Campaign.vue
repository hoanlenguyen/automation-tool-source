<template>
  <section class="section is-main-section">
    <b-table
      :data="data"
      :loading="isLoading"
      
      backend-pagination
      :total="totalItems"
      :per-page="filter.rowsPerPage"      
      :pagination-simple="false"
      pagination-position="bottom"
      
      backend-sorting
      :default-sort="filter.sortBy"
      :default-sort-direction="filter.sortDirection"
      :sort-icon="sortIcon"
      :sort-icon-size="sortIconSize"
      @sort="onSort"

      aria-next-label="Next page"
      aria-previous-label="Previous page"
      aria-page-label="Page"
      aria-current-label="Current page"           
      :pagination-order="paginationOrder"   
      :debounce-page-input="200"
    >
      <b-table-column
        field="Name"
        label="Name"
        sortable        
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.name}}
      </b-table-column>

      <b-table-column
        field="StartDate"
        label="Start Date"
        sortable
        v-slot="props"
        header-class="is-size-7"
        cell-class="is-size-6"
        width="250px">
       {{ props.row.startDate | dateTime('DD-MM-YYYY') }} 
      </b-table-column>

      <b-table-column
        field="Brand"
        label="Brand"
        sortable
        v-slot="props"
        header-class="is-size-7"
        width="150px">
       {{ props.row.brand }} 
      </b-table-column>

      <b-table-column
        field="Channel"
        label="Channel"
        sortable
        v-slot="props"
        header-class="is-size-7"
        width="150px">
       {{ props.row.channel }} 
      </b-table-column>
      
      <b-table-column
        field="Amount"
        label="Amount"
        sortable
        v-slot="props"
        header-class="is-size-7"
        width="150px">
       {{ props.row.amount }} 
      </b-table-column>

      <b-table-column
        field="PointRangeFrom"
        label="Point Range From"
        sortable
        v-slot="props"
        header-class="is-size-7"
        width="200px">  
       {{ props.row.pointRangeFrom }} 
      </b-table-column>

      <b-table-column
        field="PointRangeTo"
        label="Point Range To"
        sortable
        v-slot="props"
        header-class="is-size-7"
        width="200px">      
       {{ props.row.pointRangeTo }} 
      </b-table-column>
       
      <b-table-column
        field="exportTimesFrom"
        label="Export Time From"
        sortable
        v-slot="props"
        header-class="is-size-7"
        width="300px">
       {{ props.row.exportTimesFrom}} 
      </b-table-column>

      <b-table-column
        field="exportTimesFrom"
        label="Export Times To"
        sortable
        v-slot="props"
        header-class="is-size-7"
        width="300px">
       {{ props.row.exportTimesTo }} 
      </b-table-column>

      <b-table-column
        field="CreationTime"
        label="CreationTime"
        sortable
        v-slot="props"
        header-class="is-size-7"
        width="300px">
       {{ props.row.creationTime | dateTime }} 
      </b-table-column>

      <b-table-column
        field="Edit"
        label="Edit"        
        v-slot="props"
        width="120px">        
        <b-button 
          title="edit"          
          class="button mr-5"
          @click="editModel(props.row)" 
          style="padding: 0; border: none; background: none;">
          <b-icon
            icon="pencil"
            type="is-info">
          </b-icon>
        </b-button> 
        <b-button 
          title="delete"          
          class="button has-text-grey"
          @click="deleteSelectedModel(props.row.id)" 
          style="padding: 0; border: none; background: none;">
          <b-icon
            icon="delete">
          </b-icon>
        </b-button>       
      </b-table-column>

      <template #empty>
        <div class="has-text-centered">No records</div>
      </template>
      <div slot="subheading" class="is-flex 
        is-flex-direction-row
        is-align-items-center
        is-flex-wrap-wrap">header
      </div>
      <div slot="footer" class="is-flex 
        is-flex-direction-row
        is-align-items-center
        is-flex-wrap-wrap">
        <b-button
            label="Create"
            type="is-info"
            class="mr-4"
            icon-left="note-plus"
            @click="isModalActive=true"
          />
        
        <b-button
            label="Reset"
            type="is-light"
            class="mr-4"
            icon-left="reload"
            @click="resetFilter"
        />
        <span class="has-text-weight-normal mr-4">Total count: {{totalItems}}</span>
        <b-select v-model="filter.rowsPerPage"  @input="onChangePageSize" class="mr-4">
            <option v-for="i in pageOptions" :value="i" :key="i">{{`${i} per page`}}</option>        
        </b-select>
        <b-pagination
            :total="totalItems"
            v-model="filter.page"
            :range-before="1"
            :range-after="1"
            :order="`is-right`"
            :size="``"
            :simple="false"
            :rounded="false"
            :per-page="filter.rowsPerPage"
            :icon-prev="`chevron-left`"
            :icon-next="`chevron-right`"
            aria-next-label="Next page"
            aria-previous-label="Previous page"
            aria-page-label="Page"
            aria-current-label="Current page"
            :page-input="true"
            :page-input-position="``"
            :debounce-page-input="``"
            @change="onChangePageNumber">
        </b-pagination>        
      </div>
    </b-table>
    <b-modal v-model="isModalActive" trap-focus has-modal-card full-screen :can-cancel="false" scroll="keep">
      <div class="modal-card">
        <header class="modal-card-head">
          <p class="modal-card-title">{{model.id==0?'Create':'Update'}}</p>                 
        </header>
        <section class="modal-card-body">
          <div class="columns">
            <b-field label="Name" class="column is-3">
              <b-input
                type="Text"
                v-model="model.name"
                required>
              </b-input>
            </b-field>

            <b-field label="Start Date" class="column is-3">
              <b-datepicker
              icon="calendar-today"
              locale="en-SG"
              v-model="startDate"
              editable>
              </b-datepicker>
            </b-field> 

            <b-field label="Brand" class="column is-3">
              <b-input
                type="Text"
                v-model="model.brand"
                required>
              </b-input>
            </b-field>

            <b-field label="Channel" class="column is-3">
              <b-input
                type="Text"
                v-model="model.channel"
                required>
              </b-input>
            </b-field>          
          </div>

          <div class="columns">
            <b-field label="Amount" class="column is-3">
              <b-input
                type="Number"
                v-model="model.amount"
                required>
              </b-input>
            </b-field>

            <b-field label="Point Range From" class="column is-3">
              <b-input
                type="Number"
                v-model="model.pointRangeFrom"
                required>
              </b-input>
            </b-field>

            <b-field label="Point Range To" class="column is-3">
              <b-input
                type="Number"
                v-model="model.pointRangeTo"
                required>
              </b-input>
            </b-field>

            <b-field label="Remarks" class="column is-3">
              <b-input
                type="Text"
                v-model="model.remarks">
              </b-input>
            </b-field>          
          </div>

        <div class="columns">
          <b-field label="Export Times From" class="column is-3">
            <b-input
              type="Number"
              v-model="model.exportTimesFrom"
              required>
            </b-input>
          </b-field> 

          <b-field label="Export Times To" class="column is-3">
           <b-input
              type="Number"
              v-model="model.exportTimesTo"
              required>
            </b-input>
          </b-field> 

          <b-field label="Campaign Action" class="column is-3" v-if="model.id">
            <b-button
            label="Assign Campaign"
            type="is-primary"
            @click="isAssignCampaignModalActive= true"
            :loading="isLoadingAssignCampaign"/>
          </b-field> 
        </div>                 
        </section>
        <footer class="modal-card-foot">
            <b-button label="Close" @click="cancelCreateOrUpdate" />
            <b-button :label="model.id==0?'Create and Assign':'Update'" type="is-primary" @click="createOrUpdateModel"/>
        </footer>
    </div>
    </b-modal>

    <b-modal v-model="isDeleteModalActive" trap-focus has-modal-card auto-focus :can-cancel="false" scroll="keep">
      <div class="modal-card" style="width:300px">
        <header class="modal-card-head">
          <p class="modal-card-title">Are you sure to delete this data?</p>                 
        </header>
        <footer class="modal-card-foot">
          <b-button label="Cancel" @click="isDeleteModalActive=false; selectedId=null" />
          <b-button label="Confirm" type="is-danger is-dark" @click="deleteData"/>
        </footer>
        </div>
    </b-modal>

    <b-modal v-model="isAssignCampaignModalActive" trap-focus has-modal-card auto-focus :can-cancel="false" scroll="keep">
      <div class="modal-card" style="width:500px">
        <header class="modal-card-head">
          <p class="modal-card-title">Are you sure to assign this campaign to customers?</p>                 
        </header>
        <footer class="modal-card-foot">
          <b-button label="Cancel" @click="isAssignCampaignModalActive=false" />
          <b-button label="Confirm" type="is-info" @click="assignCampaign();"/>
        </footer>
        </div>
    </b-modal>
  </section>
</template>
<script>
import moment from "moment";
import { getDetail, getList, createOrUpdate, deleteData, assignCampaign  } from "@/api/campaign";
export default {
  name:"campaign",
  created() {
    this.getList();
  },
  data() {
    return {
      data: [],
      totalItems:0,
      isLoading:false,
      isPaginationSimple: false,
      isPaginationRounded: false,
      paginationPosition: "bottom",
      defaultSortDirection: "desc",
      sortIcon: "arrow-up",
      sortIconSize: "is-small",
      currentPage: 1,
      hasInput: false,
      paginationOrder: "is-right",
      inputPosition: "is-input-left",
      inputDebounce: "",
      pageOptions:[20,50,100],
      importTimeFrom:null,
      importTimeTo:null,
      sources:[],
      searchSource:null,
      filter:{
        page:1,
        rowsPerPage:20,
        sortBy:'Id',
        sortDirection:'desc',
        keyword:null,
        status:null
      },
      defaultFilter:{
        page:1,
        rowsPerPage:20,
        sortBy:'Id',
        sortDirection:'desc',
        keyword:null,
        status:null
      },
      model:{
        name:null,
        startDate:null,
        exportTimesFrom:0,
        exportTimesTo:0,    
        brand:null,
        channel:null,
        amount:0,
        pointRangeFrom:0,
        pointRangeTo:0,
        remarks:null,
        id:0
      },
      defaultModel:{
        name:null,
        startDate:null,
        exportTimesFrom:0,
        exportTimesTo:0,   
        brand:null,
        channel:null,
        amount:0,
        pointRangeFrom:0,
        pointRangeTo:0,
        remarks:null,
        id:0
      },
      isModalActive:false,
      isDeleteModalActive:false,
      isLoadingAssignCampaign:false,
      isAssignCampaignModalActive:false,
      selectedId:null,
      startDate:null,
    };
  },
  watch: {},
  computed: { },
  methods: {
    resetFilter() {
      this.filter = { ...this.defaultFilter };       
      this.getList();
    },
    onChangePageSize(){
      this.filter.page = 1;
      this.getList();
    },
    onChangePageNumber(page){
      this.getList();
    }, 
    onSort(field, order) {
      this.filter.sortBy = field
      this.filter.sortDirection = order
      this.getList();
    },
    getList() {
      this.isLoading = true;     

      getList(this.filter)
        .then((response) => {
          if (response.status == 200 && response.data) {
            const result =  response.data;
            this.totalItems = result.totalItems;
            this.data = result.items;             
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    closeModalDialog(){
      this.model= {...this.defaultModel};
      this.startDate= null;
      this.isModalActive= false;
    },
    cancelCreateOrUpdate(){
      this.closeModalDialog();
    },
    createOrUpdateModel(){
      if(this.startDate){
        this.model.startDate = `${this.startDate.getFullYear()}-${('0' + (this.startDate.getMonth()+1)).slice(-2)}-${('0' + this.startDate.getDate()).slice(-2)}`;
      }
      createOrUpdate(this.model)
      .then((response) => {
        if (response.status == 200) {
          this.$buefy.snackbar.open({
              message: `${this.model.id==0?'Create and assign':'Update'} campaign successfully!`,
              queue: false,
            });
          }
        })
      .catch((error) => {})
      .finally(() => {
        this.closeModalDialog();
        this.getList();
      });
    },
    editModel(input){
      this.model= {...input};
      this.startDate=this.model.startDate? moment(this.model.startDate,'YYYY-MM-DD').toDate():null;
      this.isModalActive= true;
    },
    deleteData(){
      deleteData(this.selectedId)
      .then((response) => {
        if (response.status == 200) {
          this.$buefy.snackbar.open({
              message: `Delete data successfully!`,
              queue: false,
            });
          }
        })
      .catch((error) => {})
      .finally(() => {
        this.isDeleteModalActive=false;
        this.selectedId= null;
        this.getList();
      });
    },
    deleteSelectedModel(id){
      if(id){
        this.isDeleteModalActive=true;
        this.selectedId= id;
      }
    },
    assignCampaign() {
      this.isLoadingAssignCampaign = true;
      this.isAssignCampaignModalActive=false;
      if(this.startDate){
        this.model.startDate = `${this.startDate.getFullYear()}-${('0' + (this.startDate.getMonth()+1)).slice(-2)}-${('0' + this.startDate.getDate()).slice(-2)}`;
      }
      assignCampaign(this.model)
        .then((response) => {
          if (response.status == 200) {
            this.$buefy.snackbar.open({
            message: this.model.id?'Assign campaign successfully!':'Create and assign new campaign successfully!',
            queue: false,
            });

            if(!this.model.id){
              let data = response.data;
              this.model.id=data.id
            }            
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          this.isLoadingAssignCampaign = false;
        });
    },
  }
};
</script>
