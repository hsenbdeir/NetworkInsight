import React from "react";
import logo from "../../assets/images/logo.png";
import "./header.scss";

export default function Header() {
  return (
    <div className="header">
      <img src={logo} alt="ED"></img>
      <div className="app-name">Network Insight</div>
      <div className="menu-right">
        <div className="username">{"Hussein Bdeir"} </div>
      </div>
    </div>
  );
}
