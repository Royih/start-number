import React, { Component } from "react";
import { CssBaseline } from "@material-ui/core";
import { Route } from "react-router";
import { Layout } from "./components/Layout";
import { Home } from "./components/Home";
import { FetchData } from "./components/FetchData";
import { ThemeContextProvider } from "./infrastructure/ThemeContextProvider";

import "./custom.css";

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <ThemeContextProvider>
        <CssBaseline />
        <Layout>
          <Route exact path="/" component={Home} />
          <Route path="/fetch-data" component={FetchData} />
        </Layout>
      </ThemeContextProvider>
    );
  }
}
