import React from "react";
import { Container } from "reactstrap";
import NavBar from "./NavBar";
import { Typography, Box } from "@material-ui/core";
import packageJson from "src/../package.json";
const globalAny: any = global;
globalAny.appVersion = packageJson.version;

export const Layout = (props: any) => {
  function Copyright() {
    return (
      <Typography variant="body2" color="textSecondary" align="center">
        Signup! ©{new Date().getFullYear()}. Ver: {globalAny.appVersion}
      </Typography>
    );
  }

  return (
    <div>
      <NavBar></NavBar>
      <Container>
        {props.children}
        <Box mt={8}>
          <Copyright />
        </Box>
      </Container>
    </div>
  );
};
