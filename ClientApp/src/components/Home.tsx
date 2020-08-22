import React, { useEffect, useState, useContext } from "react";
import { ActiveEventDto } from "src/models";
import { ApiContext } from "src/infrastructure/ApiContextProvider";
import { useHistory } from "react-router-dom";

export const Home = () => {
  const api = useContext(ApiContext);
  const [events, setEvents] = useState([] as ActiveEventDto[]);
  const history = useHistory();

  useEffect(() => {
    const loadData = async () => {
      const data = await api.get<ActiveEventDto[]>("anonymous/listEvents");
      setEvents(data);
    };
    loadData();
  }, [api]);

  return (
    <div>
      <h1>Welcome to Signup!</h1>
      <p>Your extremly simple signup-app. Select your event below:</p>
      <ul>
        {events.map((ev: ActiveEventDto) => (
          <li
            key={ev.tenantKey}
            onClick={() => {
              history.push("/" + ev.tenantKey);
            }}
          >
            {ev.name}
          </li>
        ))}
      </ul>
    </div>
  );
};
