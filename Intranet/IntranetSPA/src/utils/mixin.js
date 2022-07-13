import XLSX from "xlsx-js-style";
import moment from "moment";
var myMixin = {
    methods: {
        exportExcelData(data, excelName, wch = null, isAddHeader=true) {
            const limitedSize= 1000000;
            var totalCount= data.length;
            var totalSheet=Math.ceil(totalCount/limitedSize);
            var wb = XLSX.utils.book_new(); // make Workbook of Excel
            for (let index = 0; index < totalSheet; index++) {
                let endRange=(index+1)*limitedSize;
                if(index==totalSheet-1)
                endRange = totalCount;
                
                var sheetData= data.slice(index*limitedSize, endRange);
                var workSheet =isAddHeader? XLSX.utils.json_to_sheet(sheetData): XLSX.utils.json_to_sheet(sheetData, {skipHeader: 1});
                if (wch && sheetData && sheetData.length) {
                    var size = Object.keys(data[0]).length;
                    if (size) {
                    var wscols = Array(size).fill({ wch: wch });
                    workSheet["!cols"] = wscols;
                    }
                }
                XLSX.utils.book_append_sheet(wb, workSheet, `Page-${index+1}`); // sheetAName is name of Worksheet                
            }           
          
            XLSX.writeFile(
                wb,
                `${excelName}_${moment(new Date()).format("DD/MM/YYYY hh:mm:ss")}.xlsx`
            ); // name of the file is 'book.xlsx'
        },
    }
}
export default myMixin;