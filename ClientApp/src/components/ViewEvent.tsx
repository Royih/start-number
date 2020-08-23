import React, { useEffect, useState, useContext } from "react";
import { SignUpsForEventDto } from "src/models";
import { ApiContext } from "src/infrastructure/ApiContextProvider";
import { useParams } from "react-router-dom";
import { Table, TableContainer, TableHead, TableRow, TableCell, TableBody, IconButton } from "@material-ui/core";
import StartNumberIcon from "@material-ui/icons/Label";
import { saveAs } from "file-saver";

export const ViewEvent = () => {
  const api = useContext(ApiContext);
  const [signups, setSignups] = useState([] as SignUpsForEventDto[]);
  const { eventId } = useParams();

  useEffect(() => {
    const loadData = async () => {
      setSignups(await api.get<SignUpsForEventDto[]>("signup/listSignups/" + eventId));
    };
    loadData();
  }, [api, eventId]);

  const downloadStartNumber = async (personId: string, firstName: string) => {
    const data = await api.get<Blob>("signup/downloadStartNumber/" + eventId + "/" + personId, true);
    saveAs(data, `Start_Number_${firstName}.pdf`);
  };

  return (
    <div>
      <TableContainer>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>#</TableCell>
              <TableCell>First name</TableCell>
              <TableCell>Surname</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Allow contact</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {signups &&
              signups.map((signup: SignUpsForEventDto) => (
                <TableRow key={signup.startNumber} hover>
                  <TableCell>{signup.startNumber}</TableCell>
                  <TableCell>{signup.firstName}</TableCell>
                  <TableCell>{signup.surName}</TableCell>
                  <TableCell>{signup.email}</TableCell>
                  <TableCell>{signup.allowUsToContactPersonByEmail ? "Yes" : "No"}</TableCell>
                  <TableCell>
                    <IconButton
                      onClick={() => {
                        downloadStartNumber(signup.personId, signup.firstName);
                      }}
                    >
                      <StartNumberIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
          </TableBody>
        </Table>
      </TableContainer>
    </div>
  );
};
