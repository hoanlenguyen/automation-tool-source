<template>
  <section class="section is-main-section">
    <div class="columns is-desktop">
      <div class="column is-2-desktop">
        <multiselect
            ref="multiSelectLeaveHistoryBrand"
            v-model="selectBrand"
            tag-placeholder=""
            placeholder="Select brand"             
            :options="brands"
            label="name"
            track-by="id"
            :multiple="false"
            :taggable="false"
            :close-on-select="true"
            :clear-on-select="true"
            selectLabel=""
            deselectLabel="Remove"
            @select="(selectedOption, id)=>{ 
              filter.brandId=selectedOption.id; getList(); }"
            @remove="(removedOption, id)=>{ filter.brandId=null; getList(); }">
            <!-- <template slot="tag" slot-scope="{ option, remove }">
              <span class="custom__tag"><span>{{ option.name }}</span>
              <span class="custom__remove" @click="remove(option)">❌</span></span>
            </template>
            <template slot="clear" slot-scope="props">
              <div class="multiselect__clear" v-if="selectBrand" @mousedown.prevent.stop="selectBrand=null"></div>
            </template> -->
            <span  slot="noResult">No result found</span>
        </multiselect>
      </div>
      <div class="column is-2-desktop">
        <multiselect
            ref="multiSelectLeaveHistoryYear"
            v-model="selectYear"
            tag-placeholder=""
            placeholder="Select year"             
            :options="years"           
            :multiple="false"
            :taggable="false"
            :close-on-select="true"
            :clear-on-select="true"
            selectLabel=""
            deselectLabel="Remove"
            @select="(selectedOption, id)=>{onSelectDateFilter(selectedOption,selectMonth, true); }"
            @remove="(removedOption, id)=>{onSelectDateFilter(null, selectMonth, true); }">           
            <span  slot="noResult">No result found</span>
        </multiselect>     
      </div>
      <div class="column is-2-desktop">
        <multiselect
            ref="multiSelectLeaveHistoryMonth"
            v-model="selectMonth"
            tag-placeholder=""
            placeholder="Select month"             
            :options="months"           
            :multiple="false"
            :taggable="false"
            :close-on-select="true"
            :clear-on-select="true"
            selectLabel=""
            deselectLabel="Remove"
            @select="(selectedOption, id)=>{onSelectDateFilter(selectYear,selectedOption, true); }"
            @remove="(removedOption, id)=>{onSelectDateFilter(selectYear, null,true); }">          
            <span  slot="noResult">No result found</span>
        </multiselect>         
      </div>
      <div class="column is-2-desktop">
        <b-button
          label="Reset"
          type="is-light"
          class="mr-4"
          :size="$isMobile()?'is-small':''"
          icon-left="reload"
          @click="resetFilter"
        />
      </div>      
    </div>

    <b-table
      :data="data"
      :loading="isLoading"      
      backend-pagination
      backend-sorting
      :default-sort="filter.sortBy"
      :default-sort-direction="filter.sortDirection"
      :sort-icon="sortIcon"
      :sort-icon-size="sortIconSize"
      @sort="onSort"
      :debounce-page-input="200"
      mobile-cards
      narrowed
    >
      <b-table-column
        field="departmentId"
        label="Dept"
        sortable        
        width="200px"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      {{ props.row.department}}
      </b-table-column>

      <b-table-column
        field="employee.Name"
        label="Name"
        sortable        
        width="200px"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      {{ props.row.employeeName}}
      </b-table-column>

      <b-table-column
        field="employee.employeeCode"
        label="Employee ID"
        sortable        
        width="200px"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      {{ props.row.employeeCode}}
      </b-table-column>

      <b-table-column
        field="employee.rankId"
        label="Rank"
        sortable        
        width="200px"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      {{ props.row.rank}}
      </b-table-column>

      <b-table-column
        field="BrandId"
        label="Brand"
        width="200px"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      <p v-for="(brandName,index) in props.row.brands" :key="index">{{brandName}}</p> 
      </b-table-column>

      <b-table-column
        field="sumDaysOfPaidMCs"
        label="Paid MCs"
        width="200px"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      {{ props.row.sumDaysOfPaidMCs}}
      </b-table-column> 
 
      <b-table-column
        field="sumDaysOfPaidOffs"
        label="Paid Offs"
        width="200px"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      {{ props.row.sumDaysOfPaidOffs}}
      </b-table-column>
 
      <b-table-column
        field="sumHoursOfExtraPay"
        label="Extra OTs(hours)"
        width="200px"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      {{ props.row.sumHoursOfExtraPay}}
      </b-table-column>

      <b-table-column
        field="sumDaysOfExtraPay"
        label="Extra day"
        width="200px"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      {{ props.row.sumDaysOfExtraPay}}
      </b-table-column>

      <b-table-column
        field="sumDaysOfDeduction"
        label="Unpaid Leaves (Amount of day)"
        width="300"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      {{ props.row.sumDaysOfDeduction}}
      </b-table-column>

      <b-table-column
        field="sumHoursOfDeduction"
        label="Late"
        width="200px"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      {{ props.row.sumHoursOfDeduction}}
      </b-table-column>

      <b-table-column
        field="lateAmount"
        label="Late amount"
        width="200px"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      {{ props.row.lateAmount}}
      </b-table-column>

      <b-table-column
        field="fines"
        label="Fines"
        width="200px"
        header-class="is-size-7 customTableBorderHeader"
        :cell-class="$isMobile()?'customTableCellOnMobile':'customTableCell'"
        v-slot="props"
      >       
      {{ props.row.fines}}
      </b-table-column>      

      <template #empty>
        <div class="has-text-centered">No records</div>
      </template>
      
      <div slot="footer" class="is-flex 
        is-flex-direction-row
        is-align-items-center
        is-flex-wrap-wrap">
        <!-- <b-button
            label="Create"
            type="is-info"
            class="mr-4"
            :size="$isMobile()?'is-small':''"
            icon-left="note-plus"
            @click="isModalActive=true"
            v-if="canCreate"
          /> -->
        <b-button
            label="Reset"
            type="is-light"
            class="mr-4"
            :size="$isMobile()?'is-small':''"
            icon-left="reload"
            @click="resetFilter"
        />
        <span :class="$isMobile()?'is-size-7 has-text-weight-normal mr-4': 'has-text-weight-normal mr-4'">Total count: {{totalItems}}</span>
        <b-select :size="$isMobile()?'is-small':''" v-model="filter.rowsPerPage"  @input="onChangePageSize" class="mr-4">
            <option v-for="i in pageOptions" :value="i" :key="i">{{`${i} per page`}}</option>        
        </b-select>
        <b-pagination
            :total="totalItems"
            v-model="filter.page"
            :range-before="1"
            :range-after="1"
            :order="`is-right`"
            :size="$isMobile()?'is-small':''"
            :simple="false"
            :rounded="false"
            :per-page="filter.rowsPerPage"
            :icon-prev="`chevron-left`"
            :icon-next="`chevron-right`"
            aria-next-label="Next page"
            aria-previous-label="Previous page"
            aria-page-label="Page"
            aria-current-label="Current page"
            :page-input="!$isMobile()"
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
                <b-button label="Close" @click="cancelCreateOrUpdate" />
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
import Multiselect from "vue-multiselect";
import { getDetail, getList, createOrUpdate, deleteData, getBrandDropdownByUser  } from "@/api/leaveHistory";
export default {
  name:"LeaveHistory",
  components: { Multiselect },
  created() {
    this.getBrandDropdownByUser();
    this.onSelectDateFilter(this.selectYear);
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
        status:null,
        brandId:null
      },
      defaultFilter:{
        page:1,
        rowsPerPage:20,
        sortBy:'Id',
        sortDirection:'desc',
        keyword:null,
        status:null,
        brandId:null
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
      selectBrand:null,
      brands:[],
      selectMonth:null,
      selectYear:new Date().getFullYear(),
      months:["January","February","March","April","May","June","July",
            "August","September","October","November","December"]
    };
  },
  watch: {},
  computed: {
    years(){
      var date = new Date();
      var currentYear= date.getFullYear();
      var years=[];
      for (let index = 2022; index <= currentYear; index++) {
        years.push(index);
      }
      return years;
    }, 
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
      this.selectYear=new Date().getFullYear();
      this.selectMonth= null;
      this.filter = { ...this.defaultFilter }; 
      this.onSelectDateFilter(this.selectYear);
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
    onSelectDateFilter(selectYear= null, selectMonth=null, reloadData=false){
      if(selectYear==null && selectMonth==null){
        this.filter.fromTime= null;
        this.filter.toTime= null;
      }else{         
        let year= selectYear?selectYear:new Date().getFullYear();
        let monthIndex= this.months.findIndex(p=>p==selectMonth);
        var firstDay = new Date(year,monthIndex>=0? monthIndex:0, 1);
        var lastDay = new Date(year, monthIndex>=0? monthIndex+1:12, 0);
        this.filter.fromTime=this.convertDateToString(firstDay);
        this.filter.toTime=this.convertDateToString(lastDay);
      }
      if(reloadData){
        this.filter.page = 1;
        this.getList(); 
      }      
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
    },
    getBrandDropdownByUser(){
      getBrandDropdownByUser()
      .then((response) => {
        if (response.status == 200) {
          this.brands= [... response.data]; 
          }
        })
      .catch((error) => {
          this.notifyErrorMessage(error)
        })
      .finally(() => {});
    },
  }
};
</script>
