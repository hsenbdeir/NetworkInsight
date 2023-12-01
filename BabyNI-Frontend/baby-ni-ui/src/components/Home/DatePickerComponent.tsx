// DatePickerComponent.jsx
import React from 'react';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';

interface DatePickerComponentProps {
  startDate: Date | undefined;
  endDate: Date | undefined;
  onChangeStart: (date: Date) => void;
  onChangeEnd: (date: Date) => void;
  placeholderText: string;
}

const DatePickerComponent: React.FC<DatePickerComponentProps> = ({
  startDate,
  endDate,
  onChangeStart,
  onChangeEnd,
  placeholderText,
}) => {
  console.log('Selected Start Date:', startDate);
  console.log('Selected End Date:', endDate);

  return (
    <div style={{display:'flex',flexDirection:'row'}}>
      <div style={{margin : '20px'}}>
      <DatePicker
        selected={startDate}
        onChange={(date: Date) => onChangeStart(date)}
        selectsStart
        startDate={startDate}
        endDate={endDate}
        placeholderText={placeholderText}

      />
</div>
<div style={{margin : '20px'}}>
      <DatePicker
        selected={endDate}
        onChange={(date: Date) => onChangeEnd(date)}
        selectsEnd
        startDate={startDate}
        endDate={endDate}
        minDate={startDate}
        placeholderText={placeholderText}
      />
      </div>
    </div>

  );
};

export default DatePickerComponent;
