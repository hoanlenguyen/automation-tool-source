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
        field="departmentId"
        label="Dept"
        sortable        
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.department}}
      </b-table-column>

      <b-table-column
        field="employee.Name"
        label="Name"
        sortable        
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.employeeName}}
      </b-table-column>

      <b-table-column
        field="employee.employeeCode"
        label="Employee ID"
        sortable        
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.employeeCode}}
      </b-table-column>

      <b-table-column
        field="employee.rankId"
        label="Rank"
        sortable        
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.rank}}
      </b-table-column>

      <b-table-column
        field="BrandId"
        label="Brand"
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.brand}}
      </b-table-column>

      <b-table-column
        field="sumDaysOfPaidMCs"
        label="Paid MCs"
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.sumDaysOfPaidMCs}}
      </b-table-column> 
 
      <b-table-column
        field="sumDaysOfPaidOffs"
        label="Paid Offs"
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.sumDaysOfPaidOffs}}
      </b-table-column>
 
      <b-table-column
        field="sumHoursOfExtraPay"
        label="Rxtra OTs(hours)"
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.sumHoursOfExtraPay}}
      </b-table-column>

      <b-table-column
        field="sumDaysOfExtraPay"
        label="Extra day"
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.sumDaysOfExtraPay}}
      </b-table-column>

      <b-table-column
        field="sumDaysOfDeduction"
        label="Unpaid of Leaves (Amount of day)"
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.sumDaysOfDeduction}}
      </b-table-column>

      <b-table-column
        field="sumHoursOfDeduction"
        label="Late"
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.sumHoursOfDeduction}}
      </b-table-column>

      <b-table-column
        field="lateAmount"
        label="Late amount"
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.lateAmount}}
      </b-table-column>

      <b-table-column
        field="fines"
        label="Fines"
        width="200px"
        header-class="is-size-7"
        v-slot="props"
      >       
      {{ props.row.fines}}
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
            v-if="canCreate"
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
    <b-modal v-model="isModalActive" trap-focus has-modal-card :can-cancel="false" width="1200" scroll="keep">
      <form action="">
        <div class="modal-card">
            <header class="modal-card-head">
                <p class="modal-card-title">{{model.id==0?'Create':'Update'}}</p>                 
            </header>
            <section class="modal-card-body">
              <b-field>
                <b-switch v-model="model.status" type='is-info'>{{model.status?'Active':'Inactive'}}</b-switch>
              </b-field>
              <b-field label="Name">
                  <b-input
                    type="Text"
                    v-model.trim="model.name"
                    placeholder="Name...."
                    required>
                  </b-input>
              </b-field>                 
            </section>
            <footer class="modal-card-foot">
                <b-button label="Cancel" @click="cancelCreateOrUpdate" />
                <b-button :label="model.id==0?'Create':'Update'" type="is-primary" @click="createOrUpdateModel"/>
            </footer>
        </div>
      </form>
    </b-modal>

    <b-modal v-model="isDeleteModalActive" trap-focus has-modal-card auto-focus :can-cancel="false" scroll="keep">
      <div class="modal-card" style="width:300px">
        <header class="modal-card-head">
          <p class="modal-card-title">Are you sure to delete this data</p>                 
        </header>
        <footer class="modal-card-foot">
          <b-button label="Cancel" @click="isDeleteModalActive=false; selectedId=null" />
          <b-button label="Confirm" type="is-info" @click="deleteData"/>
        </footer>
        </div>
    </b-modal>
  </section>
</template>
<script>
import { getDetail, getList, createOrUpdate, deleteData  } from "@/api/leaveHistory";
export default {
  name:"bank",
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
        status:true,
        id:0
      },
      defaultModel:{
        name:null,
        status:true,
        id:0
      },
      isModalActive:false,
      isDeleteModalActive:false,
      selectedId:null,
    };
  },
  watch: {},
  computed: {
    canCreate() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "StaffRecord.Create"
        )
      );
    },
    canUpdate() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "StaffRecord.Update"
        )
      );
    },
    canDelete() {
      return (
        this.$store.state.userPermissions &&
        this.$store.state.userPermissions.includes(
          "StaffRecord.Delete"
        )
      );
    }
   },
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
          this.notifyErrorMessage(error)
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    closeModalDialog(){
      this.model= {...this.defaultModel};
      this.isModalActive= false;
    },
    cancelCreateOrUpdate(){
      this.closeModalDialog();
    },
    createOrUpdateModel(){
      createOrUpdate(this.model)
      .then((response) => {
        if (response.status == 200) {
          this.$buefy.snackbar.open({
              message: `${this.model.id==0?'Create':'Update'} successfully!`,
              queue: false,
            });
          }
        })
      .catch((error) => {
          this.notifyErrorMessage(error)
        })
      .finally(() => {
        this.closeModalDialog();
        this.getList();
      });
    },
    editModel(input){
      this.model= {...input};
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
      .catch((error) => {
          this.notifyErrorMessage(error)
        })
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
    }
  }
};
</script>
