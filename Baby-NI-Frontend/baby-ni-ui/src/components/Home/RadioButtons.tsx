import React from 'react';
import { IFiterValues } from '../../models/AggregatedData/IFiterValues';

interface RadioButtonsProps {
  filterValues: IFiterValues;
  onFilterChange: (filterValues: IFiterValues) => void;
}

const RadioButtons: React.FC<RadioButtonsProps> = ({ 
    filterValues,
     onFilterChange 
}) => {
  const handleRadioChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    onFilterChange({ ...filterValues, [name]: value });
    
  };

  return (
    <div>
      <label>
        Global Filter:
        <input
          type="radio"
          name="globalFilterValue"
          value="NeType"
          checked={filterValues.globalFilterValue === 'NeType'}
          onChange={handleRadioChange}
        />
        NeType
        <input
          type="radio"
          name="globalFilterValue"
          value="NeAlias"
          checked={filterValues.globalFilterValue === 'NeAlias'}
          onChange={handleRadioChange}
        />
        NeAlias
      </label>

      <label style = {{padding : '20px'}}>
        Date Time Filter:
        <input
          type="radio"
          name="dateTimeFilterValue"
          value="Hourly"
          checked={filterValues.dateTimeFilterValue === 'Hourly'}
          onChange={handleRadioChange}
        />
        Hourly
        <input
          type="radio"
          name="dateTimeFilterValue"
          value="Daily"
          checked={filterValues.dateTimeFilterValue === 'Daily'}
          onChange={handleRadioChange}
        />
        Daily
      </label>
    </div>
  );
};

export default RadioButtons;
