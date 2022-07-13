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
        field="ImportTime"
        label="Import Time"
        sortable        
        searchable
        width="350px"
      >
      <template #searchable="props">
       <b-field>
          <p class="control">
            <b-datepicker
              icon="calendar-today"
              locale="en-CA"
              v-model="importTimeFrom"
              editable
              size="is-small"
              placeholder="from"
              @input="onChangePageSize"
            ></b-datepicker>
          </p>
          <p class="control ml-2">
            <b-datepicker
              icon="calendar-today"
              locale="en-CA"
              v-model="importTimeTo"
              editable
              size="is-small"
              placeholder="to"
              @input="onChangePageSize"
            ></b-datepicker>
          </p>
        </b-field>
      </template>
      <template v-slot="props">{{ props.row.importTime | dateTime }}</template>
      </b-table-column>

      <b-table-column
        field="Source"
        label="Source"
        searchable 
        sortable
        width="300px">
        <template #searchable="props">        
            <b-autocomplete
              open-on-focus
              v-model="searchSource"
              :data="filteredDataArray"
              placeholder="Search..."
              icon-right="magnify"                
              @keyup.native.enter="onChangePageSize"
              @input="onSelectSource"
              clearable
              size="is-small"              
              @select="option => selected = option">
              <template #empty>No sources found</template>
            </b-autocomplete>
        </template>
        <template v-slot="props">{{ props.row.source }}</template>        
      </b-table-column>

      <b-table-column
        field="FileName"
        label="File Name"
        sortable
        v-slot="props"
        width="400px">
        {{props.row.fileName}}
      </b-table-column>

      <b-table-column
        field="TotalRows"
        label="Number of data"
        v-slot="props"
      >
        {{props.row.totalRows}}
      </b-table-column>
      <template #empty>
        <div class="has-text-centered">No records</div>
      </template>
      <div slot="footer" class="is-flex 
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
  </section>
</template>
<script>
import moment from "moment";
import { getPagingImportHistories, getSource  } from "@/api/importHistory";
export default {
  name:"ImportHistory",
  created() {
    this.getSource();
    this.getImportHistories();
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
        sortBy:'ImportTime',
        sortDirection:'desc',
        importTimeFrom:null,
        importTimeTo:null,
        source: null
      },
      defaultFilter:{
        page:1,
        rowsPerPage:20,
        sortBy:'ImportTime',
        sortDirection:'desc',
        importTimeFrom:null,
        importTimeTo:null,
        source: null
      }
    };
  },
  watch: {},
  computed: {
    filteredDataArray() {
      if(!this.searchSource) return this.sources;
      return this.sources.filter((option) => {
          return option
              .toString()
              .toLowerCase()
              .indexOf(this.searchSource.toLowerCase()) >= 0
      })
    }
  },
  methods: {
    resetFilter() {
      this.filter = { ...this.defaultFilter };
      this.importTimeFrom = null;
      this.importTimeTo = null;
      this.searchSource = null; 
      this.getImportHistories();
    },
    onChangePageSize(){
      this.filter.page = 1;
      this.getImportHistories();
    },
    onChangePageNumber(page){
      //console.log(this.filter.page);
      //console.log(page);
      this.getImportHistories();
    }, 
    onSort(field, order) {
      //console.log('field'+field);
      //console.log('order'+order);
      this.filter.sortBy = field
      this.filter.sortDirection = order
      this.getImportHistories()
    },
    onSelectSource(input){
      if(input!=null && input.length==0) {
        this.filter.source = null;
        this.filter.page = 1;
        this.getImportHistories();
      }else{
        var searchSource = input.toLowerCase();
        var findIndex = this.sources.findIndex(p=>p.toLowerCase()==searchSource);
        if(findIndex>=0){
          this.filter.source = input;
          this.filter.page = 1;
          this.getImportHistories();
        }
      }            
    },
    getImportHistories() {
      this.isLoading = true;
      const outputFormat = "YYYY-MM-DD";
      this.filter.importTimeFrom =this.importTimeFrom? moment(this.importTimeFrom).format(outputFormat):null;
      this.filter.importTimeTo =this.importTimeTo? moment(this.importTimeTo).format(outputFormat):null;
      if(!this.filter.source) this.filter.source=null;

      getPagingImportHistories(this.filter)
        .then((response) => {
          if (response.status == 200 && response.data) {
            const result =  response.data;
            this.totalItems= result.totalItems;
            this.data= result.items;
            //console.log(object);
            //this.customerList = response.data;
            //console.log(this.customerList);
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    getSource() {
      getSource()
        .then((response) => {
          if (response.status == 200) {
            this.sources = response.data;
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          //this.isLoading = false;
        });
    },
  }
};
</script>
