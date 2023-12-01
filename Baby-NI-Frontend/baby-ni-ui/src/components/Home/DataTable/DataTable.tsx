import * as React from "react";
import Box from "@mui/material/Box";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell, { tableCellClasses } from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import Paper from "@mui/material/Paper";
import { styled } from "@mui/material/styles";
import { Result } from "../../../models/User/IUsers";
import { IFiterValues } from "../../../models/AggregatedData/IFiterValues";

interface HeadCell {
  disablePadding: boolean;
  id: keyof Result;
  label: string;
  numeric: boolean;
}
// use of Styled components from Material UI.
const StyledTableCell = styled(TableCell)(({ theme }) => ({
  [`&.${tableCellClasses.head}`]: {
    backgroundColor: theme.palette.grey,
    color: theme.palette.common.black,
  },
  [`&.${tableCellClasses.body}`]: {
    fontSize: 14,
  },
  padding: "5px",
}));

const StyledTableRow = styled(TableRow)(({ theme }) => ({
  "&:nth-of-type(odd)": {
    backgroundColor: theme.palette.action.hover,
  },
  // hide last border
  "&:last-child td, &:last-child th": {
    border: 0,
  },
}));

const headCells: readonly HeadCell[] = [
  { id: "time", numeric: false, disablePadding: false, label: "Time", },
  { id: "rfInputPower", numeric: false, disablePadding: false, label: "RFInputPower", },
  { id: "maxRxLevel", numeric: false, disablePadding: false, label: "maxRxLevel", },
  { id: "rsL_Deviation", numeric: true, disablePadding: false, label: "rsL_Deviation", },
  { id: "neType", numeric: false, disablePadding: false, label: "NeType", },
  { id: "neAlias", numeric: false, disablePadding: false, label: "NeAlias", }
];

type Props = {
  gridData: any; 
  handleChangeDataRequest: any; 
  getGridData: IFiterValues;
  // hide:boolean,
};
export default function EnhancedTable(props: Props) {
  //console.log("Inside DataTable Component");
  const { gridData } = props;
  const [ ,setResults] = React.useState<Result[]>([]);
  // const [filteredHeadCells,setFilteredHeadCells] = React.useState< HeadCell[]>([]);

 
 // console.log('Results in DataTable:', gridData);

  React.useEffect(() => {
   
  //  console.log("Inside useEffect");
  // if (hide) {
  //   setFilteredHeadCells( headCells.filter(cell => cell.id !== "neType"))
  // } else {
   
  //   setFilteredHeadCells( headCells.filter(cell => cell.id !== "neAlias"))
  // }
    setResults(gridData.results || []);
  }, [gridData.results]);

  //console.log('data in DataTable2:', gridData);
  return (
    <Box sx={{ width: "100%" }}>
      <Paper sx={{ width: "100%", mb: 2 }}>
        <TableContainer>
        <Table sx={{ minWidth: 750 }} aria-labelledby="tableTitle" size={"medium"}>
  <TableHead>
    <TableRow>
      {headCells.map((headcells: any) => (
        <StyledTableCell 
        key={headcells.id}>{headcells.label}
        </StyledTableCell>
      ))}
    </TableRow>
  </TableHead>
  <TableBody>
    {gridData.length > 0 ? (
      gridData.map((row :any , index : any) => (
        <StyledTableRow key={index}>
          {headCells.map((cell) => (
            <StyledTableCell 
            key={cell.id}>{row[cell.id]?.toString() ?? 'N/A'}
            </StyledTableCell>
          ))}
        </StyledTableRow>
      ))
    ) : (
      <StyledTableRow>
        <StyledTableCell 
        colSpan={headCells.length} align="center">No data available
        </StyledTableCell>
      </StyledTableRow>
    )}
  </TableBody>
</Table>
        </TableContainer>
      </Paper>
    </Box>
  );

}
