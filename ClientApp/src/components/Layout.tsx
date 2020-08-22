import React from "react";
import { Container } from "reactstrap";
import NavBar from "./NavBar";

export const Layout = (props: any) => {
  return (
    <div>
      <NavBar></NavBar> <Container> {props.children} </Container>{" "}
    </div>
  );
};
