import { IFiterValues } from "../models/AggregatedData/IFiterValues";
import { Result } from "../models/User/IUsers";
import { ApiBase } from "./API";
import moment from "moment";

let appBasePath = "https://localhost";

export interface IGridDataService {
  GetData: (request: IFiterValues, startDate?: Date, endDate?: Date) => Promise<Result[]>;
}

class DataService extends ApiBase implements IGridDataService {
  constructor() {
    super(appBasePath);
  }
 public GetData = async (request: IFiterValues): Promise<Result[]> => {

    try {
      const { globalFilterValue, dateTimeFilterValue } = request;

      let url = `/api/DataProvider?globalFilterValues=${globalFilterValue}&dateTimeFilterValue=${dateTimeFilterValue}`;

      //const url2 = `/api/DataProvider?globalFilterValues=${globalFilterValue}&dateTimeFilterValue=${dateTimeFilterValue}&startDate=${startDate}&endDate=${endDate}`;

      const response = await this.instance.get<Result[]>(url);
      let filteredData: Result[] = response;
      if (request.startDate && request.endDate) {
        filteredData = this.filterDataByDateRange(response, request.startDate, request.endDate);
      }

      // Access 'data' instead of 'results'
      // console.log("API Response:", wrappedResponse);
      //console.log("API Response2:", response);
      return filteredData;
    } catch (error) {
      console.error("Error fetching data:", error);
      throw error;
    }
  };
  private filterDataByDateRange(
    data: Result[],
    startDate: Date,
    endDate: Date
  ): Result[] {
    const filteredResults = data.filter((item) => {
      const itemDate = moment(item.time);
      const itemDateWithoutTime = itemDate.startOf('day');
      const startDateWithoutTime = moment(startDate).startOf('day');
      const endDateWithoutTime = moment(endDate).endOf('day');

      return itemDateWithoutTime.isSameOrAfter(startDateWithoutTime) &&
        itemDateWithoutTime.isSameOrBefore(endDateWithoutTime);
    });

    return filteredResults
  }
}
export default DataService;
