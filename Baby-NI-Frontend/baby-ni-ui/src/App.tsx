import React from "react";
import  LandingPage  from "./components/LandingPage/LandingPage";
import MainLayout from "./hoc/layout/MainLayout";
function App() {
  return (
    <div className="App">
      <MainLayout>
        <LandingPage></LandingPage>
      </MainLayout>
    </div>
  );
}

export default App;
