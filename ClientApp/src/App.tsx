import React, { useContext } from "react";
import { CssBaseline } from "@material-ui/core";
import { Route } from "react-router";
import { Layout } from "./components/Layout";
import { Home } from "./components/Home";
import { ThemeContextProvider } from "./infrastructure/ThemeContextProvider";
import "./custom.css";
import { ApiContextProvider } from "./infrastructure/ApiContextProvider";
import { UserContextProvider, UserContext, RoleTypes } from "./infrastructure/UserContextProvider";
import { Login } from "./components/Login";
import { SignUp } from "./components/SignUp";
import { SnackbarProvider } from "notistack";
import { ViewEvent } from "./components/ViewEvent";

const UserRoutes = () => {
  const currentUser = useContext(UserContext);
  if (currentUser && currentUser.hasRole && currentUser.hasRole(RoleTypes.User)) {
    return <Route exact path="/view/:eventId" component={ViewEvent} />;
  }
  return null;
};

function App() {
  return (
    <SnackbarProvider maxSnack={10}>
      <ApiContextProvider>
        <UserContextProvider>
          <ThemeContextProvider>
            <CssBaseline />
            <Layout>
              <Route exact path="/" component={Home} />
              <Route exact path="/signup/:key" component={SignUp} />
              <UserRoutes></UserRoutes>
              <Route path="/login" component={Login} />
            </Layout>
          </ThemeContextProvider>
        </UserContextProvider>
      </ApiContextProvider>
    </SnackbarProvider>
  );
}
export default App;
