import React from "react";
import { Header } from "../../components/Header";

function MainLayout({ children }: any) {
  return (
    <div>
      <Header></Header>
      <div className="main-container">{children}</div>
    </div>
  );
}
export default React.memo(MainLayout);
