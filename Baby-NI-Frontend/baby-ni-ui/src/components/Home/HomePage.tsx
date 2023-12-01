import React, { useState, useEffect } from "react";
import { DataTable } from "./DataTable";
import DataService from "../../services/DataService";
import { IFiterValues } from "../../models/AggregatedData/IFiterValues";
import { Result } from "../../models/User/IUsers";
import DatePickerComponent from './DatePickerComponent';
import 'react-datepicker/dist/react-datepicker.css';
import { format } from 'date-fns';
import RadioButtons from './RadioButtons'; 

const dataService = new DataService();
const defaultDataState: Result[] = [];
const defaultDataRequest = {
  globalFilterValue: "",
  dateTimeFilterValue: "",
  startDate: new Date("2020-03-11"),
  endDate: new Date("2020-03-10"),
};


export default function Data() {
  const [data, setData] = useState<Result[]>(defaultDataState);
  const [getDataRequest, setGetDataRequest] = useState<IFiterValues>(defaultDataRequest);
  const [startDate, setStartDate] = useState<Date>(new Date());
  const [endDate, setEndDate] = useState<Date>(new Date());
 
  const handleRadioChange = (filterValues: IFiterValues) => {
    setGetDataRequest(filterValues);
  };

  const handleChangeDataRequest = (dataRequest: IFiterValues) => {
    setGetDataRequest(dataRequest);
  };
  const handleStartDateChange = (date: Date) => {
    console.log('Start Date Changed:', date);

    setStartDate(date);
    setGetDataRequest({
      ...getDataRequest, startDate: date
    });

  };
  const handleEndDateChange = (date: Date) => {
    console.log('End Date Changed:', date);
    const formattedDate = format(date, 'yyyy-MM-dd');

    setEndDate(date);
    setGetDataRequest({
      ...getDataRequest, endDate: date
    });
  };


  useEffect(() => {
    console.log('Current startDate:', startDate);
    console.log('Current endDate:', endDate);
    const GetData = async () => {
      try {

        console.log('API Request:', getDataRequest, startDate, endDate);
        console.log('API Request2:', { ...getDataRequest, startDate, endDate });
        const response = await dataService.GetData({
          ...getDataRequest
        });

        setData(response);

      } catch (error) {
        console.error('Error fetching data:', error);
      }
    };

    GetData();
  }, [getDataRequest, startDate, endDate]);
  return (
    <div>
      <RadioButtons filterValues={getDataRequest} onFilterChange={handleRadioChange} />
      <DatePickerComponent
        startDate={startDate}
        endDate={endDate}
        onChangeStart={handleStartDateChange}
        onChangeEnd={handleEndDateChange}
        placeholderText="Select Date"
      />
      <DataTable
        gridData={data}
        handleChangeDataRequest={handleChangeDataRequest}
        getGridData={getDataRequest}
      />
      
    </div>



  );
}

