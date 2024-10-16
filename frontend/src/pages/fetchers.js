export const fetchUsers = async () => {
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_MEETING_URL}/ext-users`
    );

    if (resp.status !== 200) {
      throw await resp.text();
    }
    return await resp.json();
  } catch (e) {
    return [];
  }
};

export const fetchWoTemplates = async () => {
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_MEETING_URL}/tasklisttemplates-proto`
    );

    if (resp.status !== 200) {
      throw await resp.text();
    }
    return await resp.json();
  } catch (e) {
    return [];
  }
};

export const fetchInquiries = async () => {
  try {
    const resp = await fetch(
      `https://backend-crm.iotech.my.id/api/v1/external/inquiries`
    );

    if (resp.status !== 200) {
      throw await resp.text();
    }
    return await resp.json();
  } catch (e) {
    return [];
  }
};

export const fetchExtCrmPurchaseOrdersProtoSimple = async () => {
  try {
    const resp = await fetch(
      `https://crm-local.iotech.my.id/api/external/purchase-orders/ppic`
    );

    if (resp.status !== 200) throw await resp.text();

    const pos = await resp.json();

    return pos;
  } catch (e) {
    console.log(e);
    return null;
  }
};

export const fetchDepartments = async () => {
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_MEETING_URL}/ext-departments`
    );

    if (resp.status !== 200) {
      throw await resp.text();
    }
    return await resp.json();
  } catch (e) {
    return [];
  }
};

export const fetchJobsProtoSimple = async (props) => {
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_PPIC_URL}/jobs-proto-simple?all=${
        props?.all ?? ""
      }&withProducts=${props?.withProducts ?? ""}&withPurchaseOrders=${
        props?.withPurchaseOrders ?? ""
      }`
    );

    if (resp.status !== 200) {
      throw await resp.text();
    }
    return await resp.json();
  } catch (e) {
    return [];
  }
};

export const fetchBomLeveledsProtoSimple = async (props) => {
  try {
    const resp = await fetch(
      `${
        import.meta.env.VITE_APP_PPIC_URL
      }/bomleveleds-proto-simple?bomGenesisReferenceId=${
        props?.bomGenesisReferenceId ?? ""
      }`
    );

    if (resp.status !== 200) {
      throw await resp.text();
    }
    return await resp.json();
  } catch (e) {
    return [];
  }
};

export const fetchItems = async () => {
  try {
    const resp = await fetch(`${import.meta.env.VITE_APP_PPIC_URL}/ext-items`);

    if (resp.status !== 200) {
      throw await resp.text();
    }
    return await resp.json();
  } catch (e) {
    return [];
  }
};

export const fetchBomDetail = async (params) => {
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_PPIC_URL}/bomleveleds-proto-detailed/${
        params?.id ?? ""
      }`
    );

    if (resp.status !== 200) {
      throw await resp.text();
    }
    return await resp.json();
  } catch (e) {
    return {};
  }
};

export const fetchUser = async (params) => {
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_AUTHSERVER_URL}/users/${params?.id ?? ""}`,
      { headers: { authorization: params?.apiKey ?? "" } }
    );

    if (resp.status !== 200) {
      throw await resp.text();
    }
    return await resp.json();
  } catch (e) {
    return null;
  }
};
export const fetchInventory = async () => {
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_PPIC_URL}/ext-inventory?all=true`
    );

    if (resp.status !== 200) {
      throw await resp.text();
    }
    return await resp.json();
  } catch (e) {
    return [];
  }
};

export const getEngineeringActivitiesUrl = (params) =>
  `${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/activities?from=${
    params?.from ?? ""
  }&to=${params?.to ?? ""}${params?.excel ? `&excel=${params.excel}` : ""}${
    params?.withUserNames ? `&withUserNames=${params.withUserNames}` : ""
  }${
    params?.userId ? `&userId=${params.userId}` : ""
  }`;

export const fetchEngineeringActivities = async (params) => {
  try {
    const resp = await fetch(getEngineeringActivitiesUrl(params));
    if (resp.status !== 200) {
      throw await resp.text();
    }

    return await resp.json();
  } catch (error) {
    return [];
  }
};
export const fetchBomApprovals = async (params) => {
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/bomapprovals`
    );
    if (resp.status !== 200) {
      throw await resp.text();
    }

    return await resp.json();
  } catch (error) {
    return [];
  }
};
export const fetchBomApproval = async (params) => {
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/bomapprovals/${params?.id ?? ""}`
    );
    if (resp.status !== 200) {
      throw await resp.text();
    }

    return await resp.json();
  } catch (error) {
    return [];
  }
};

export const fetchEngineeringDetailProblems = async () => {
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/engineeringDetailProblems`
    );
    if (resp.status !== 200) {
      throw await resp.text();
    }

    return await resp.json();
  } catch (error) {
    return [];
  }
};

export const fetchEngineeringDetailProblem = async (id) => {
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/engineeringDetailProblems/${id}`
    );
    if (resp.status !== 200) {
      throw await resp.text();
    }

    return await resp.json();
  } catch (error) {
    return [];
  }
};
