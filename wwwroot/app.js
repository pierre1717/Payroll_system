
const api = '';

async function fetchEmployees(){
  const res = await fetch('/employees');
  return await res.json();
}

async function fetchPayrolls(){
  const res = await fetch('/payrolls');
  return await res.json();
}

function toCurrency(v){ return Number(v).toLocaleString(); }

async function refresh(){
  const emps = await fetchEmployees();
  const list = document.getElementById('employeesList');
  const sel = document.getElementById('empSelect');
  list.innerHTML = '';
  sel.innerHTML = '';
  emps.forEach(e=>{
    const li = document.createElement('li');
    li.innerHTML = `<div><strong>${e.firstName || ''} ${e.lastName || ''}</strong><div style="font-size:12px;color:#6b7280">${e.position||''}</div></div><div>${toCurrency(e.baseSalary)}</div>`;
    list.appendChild(li);
    const opt = document.createElement('option');
    opt.value = e.id; opt.text = `${e.firstName || ''} ${e.lastName || ''}`;
    sel.appendChild(opt);
  });
  const payrolls = await fetchPayrolls();
  const pList = document.getElementById('payrollsList');
  pList.innerHTML = '';
  payrolls.slice().reverse().slice(0,10).forEach(p=>{
    const li = document.createElement('li');
    li.textContent = `${p.period} — Net: ${toCurrency(p.netPay)} (Employee ${p.employeeId.slice(0,8)})`;
    pList.appendChild(li);
  });
}

document.getElementById('addBtn').addEventListener('click', async ()=>{
  const first = document.getElementById('first').value;
  const last = document.getElementById('last').value;
  const position = document.getElementById('position').value;
  const base = Number(document.getElementById('base').value || 0);
  const allow = Number(document.getElementById('allow').value || 0);
  const deduct = Number(document.getElementById('deduct').value || 0);
  const body = { firstName:first, lastName:last, position:position, baseSalary:base, allowances:allow, deductions:deduct };
  await fetch('/employees',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(body)});
  await refresh();
});

document.getElementById('calcBtn').addEventListener('click', async ()=>{
  const empId = document.getElementById('empSelect').value;
  const period = document.getElementById('period').value;
  const tax = Number(document.getElementById('tax').value || 0);
  const body = { employeeId: empId, period: period || undefined, taxRate: tax || undefined };
  const res = await fetch('/payrolls/calculate',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(body)});
  if(res.ok){ alert('Fiche de paie calculée et enregistrée'); await refresh(); }
  else{ alert('Erreur: ' + (await res.text())); }
});

// initial refresh
refresh();
