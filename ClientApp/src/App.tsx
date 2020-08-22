import React from "react";
import { CssBaseline } from "@material-ui/core";
import { Route } from "react-router";
import { Layout } from "./components/Layout";
import { Home } from "./components/Home";
import { ThemeContextProvider } from "./infrastructure/ThemeContextProvider";
import "./custom.css";
import { ApiContextProvider } from "./infrastructure/ApiContextProvider";
import { UserContextProvider } from "./infrastructure/UserContextProvider";
import { Login } from "./components/Login";
import { SignUp } from "./components/SignUp";
import { SnackbarProvider } from "notistack";

function App() {
  return (
    <SnackbarProvider maxSnack={10}>
      <ApiContextProvider>
        <UserContextProvider>
          <ThemeContextProvider>
            <CssBaseline />
            <Layout>
              <Route exact path="/" component={Home} />
              <Route exact path="/:key" component={SignUp} />
              <Route exact path="/login" component={Login} />
            </Layout>
          </ThemeContextProvider>
        </UserContextProvider>
      </ApiContextProvider>
    </SnackbarProvider>
  );
}
export default App;
